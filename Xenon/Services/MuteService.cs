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

        return muteResult;
    }

    /// <summary>
    /// Unbans a user from voice channels.
    /// </summary>
    /// <param name="target">The user to unban.</param>
    /// <returns>A <see cref="Result"/> indicating the outcome of the operation.</returns>
    public async Task<Result> UnVCBanUserAsync(IUser target)
    {
        UserMute? mute = mutes.FirstOrDefault(x => x.UserID == target.ID);

        if (mute is not null)
        {
            mutes.Remove(mute);
        }

        await using ModHelperContext context = await contextFactory.CreateDbContextAsync();
        mute = context.Mutes.FirstOrDefault(m => m.UserID == target.ID && m.Expiry > time.GetUtcNow());

        if (mute is null)
        {
            return Result.FromError(new NotFoundError("User is not banned from voice channels."));
        }

        await guildApi.RemoveGuildMemberRoleAsync(guildID, mute.UserID, vcBanRoleID, "Unbanned from voice channels.");
        await guildApi.RemoveGuildMemberRoleAsync(guildID, mute.UserID, quarantineRoleID, "Unbanned from voice channels.");

        return Result.Success;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var gatewayTask = gateway.RunAsync(stoppingToken);
            await ((Task)gatewayTask).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

            if (gatewayTask.Result.IsSuccess)
            {
                return;
            }
        }
    }
}
