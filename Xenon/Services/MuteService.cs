using Xenon.Data;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Gateway;
using Remora.Rest.Core;
using Remora.Results;

namespace Xenon.Services;

public sealed class MuteService
(
    TimeProvider time,
    IConfiguration config,
    LoggingService logging,
    DiscordGatewayClient gateway,
    IDiscordRestGuildAPI guildApi,
    IDbContextFactory<ModHelperContext> contextFactory
) : BackgroundService
{
    private readonly List<UserMute> mutes = [];
    private readonly Snowflake guildID = new(config.GetValue<ulong>("DiscordServerID"));
    private readonly Snowflake vcBanRoleID = new(config.GetValue<ulong>("VoiceBanRoleID"));
    private readonly Snowflake quarantineRoleID = new(config.GetValue<ulong>("QuarantineRoleID"));

    public async Task<Result> VCBanUserAsync
    (
        IUser target,
        IUser moderator,
        DateTimeOffset expiry,
        string reason
    )
    {
        UserMute mute = new()
        {
            UserID = target.ID,
            Moderator = moderator.ID,
            CreatedAt = time.GetUtcNow(),
            AppliedRoleID = vcBanRoleID,
            Expiry = expiry,
            Reason = reason,
        };

        mutes.Add(mute);

        await using ModHelperContext context = await contextFactory.CreateDbContextAsync();
        await context.Mutes.AddAsync(mute);
        await context.SaveChangesAsync();

        Result muteResult = await guildApi.AddGuildMemberRoleAsync(guildID, mute.UserID, vcBanRoleID, $"{moderator.Username}: {reason}".Truncate(50, "[...]"));

        await logging.LogActionAsync(target, moderator, expiry, reason);

        await guildApi.ModifyGuildMemberAsync(guildID, target.ID, channelID: null);

        return muteResult;
    }

    // Document the method:
    /// <summary>
    /// Quarantines a user from voice channels.
    /// </summary>
    /// <param name="target">The user to quarantine.</param>
    /// <param name="moderator">The user who issued the quarantine.</param>
    /// <param name="expiry">When the quarantine expires.</param>
    /// <param name="reason">The reason for the quarantine.</param>
    /// <returns>A <see cref="Result"/> indicating the outcome of the operation.</returns>
    public async Task<Result> VCQuarantineUserAsync
    (
        IUser target,
        IUser moderator,
        DateTimeOffset expiry,
        string reason
    )
    {
        UserMute mute = new()
        {
            UserID = target.ID,
            Moderator = moderator.ID,
            CreatedAt = time.GetUtcNow(),
            AppliedRoleID = quarantineRoleID,
            Expiry = expiry,
            Reason = reason,
        };

        mutes.Add(mute);

        await using ModHelperContext context = await contextFactory.CreateDbContextAsync();
        await context.Mutes.AddAsync(mute);
        await context.SaveChangesAsync();

        Result muteResult = await guildApi.AddGuildMemberRoleAsync(guildID, mute.UserID, quarantineRoleID, $"{moderator.Username}: {reason}".Truncate(50, "[...]"));

        await logging.LogActionAsync(target, moderator, expiry, reason);

        // Third parameter is null by default, which will kick the user
        // from the voice channel.
        await guildApi.ModifyGuildMemberAsync(guildID, target.ID, channelID: null);

        return muteResult;
    }

    /// <summary>
    /// Unbans a user from voice channels.
    /// </summary>
    /// <param name="targetID">The user to unban.</param>
    /// <returns>A <see cref="Result"/> indicating the outcome of the operation.</returns>
    public async Task<Result> UnVCBanUserAsync(Snowflake targetID)
    {
        UserMute? mute = mutes.FirstOrDefault(x => x.UserID == targetID);

        if (mute is not null)
        {
            mutes.Remove(mute);
        }

        await using ModHelperContext context = await contextFactory.CreateDbContextAsync();
        mute = context.Mutes.FirstOrDefault(m => m.UserID == targetID && m.Active);

        if (mute is null)
        {
            return Result.FromError(new NotFoundError("User is not banned from voice channels."));
        }

        await guildApi.RemoveGuildMemberRoleAsync(guildID, mute.UserID, vcBanRoleID, "Unbanned from voice channels.");
        await guildApi.RemoveGuildMemberRoleAsync(guildID, mute.UserID, quarantineRoleID, "Unbanned from voice channels.");

        mute.Active = false;

        await context.SaveChangesAsync();

        return Result.Success;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Load all active mutes from the database at startup
        await using ModHelperContext context = await contextFactory.CreateDbContextAsync(stoppingToken);
        mutes.AddRange(context.Mutes.Where(m => m.Active));

        using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            // Iterate through all active mutes
            for (var i = mutes.Count - 1; i >= 0; i--)
            {
                UserMute mute = mutes[i];

                // If the mute has expired
                if (time.GetUtcNow() < mute.Expiry)
                {
                    continue;
                }

                await UnVCBanUserAsync(mute.UserID);
                mutes.Remove(mute);
            }
        }
    }
}
