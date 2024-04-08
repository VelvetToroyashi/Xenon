using System.Runtime.CompilerServices;
using FDModHelper.Data;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Gateway;
using Remora.Rest.Core;
using Remora.Results;

namespace FDModHelper.Services;

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
    private readonly List<UserMute> _mutes = [];
    private readonly Snowflake _muteRoleID = new(config.GetValue<ulong>("XENON_MuteRoleID"));
    private readonly Snowflake _guildID = new(config.GetValue<ulong>("XENON_DiscordGuildID"));
    
    
    public async Task<Result> AddMuteAsync(IUser target, IUser moderator, DateTimeOffset expiry, string reason)
    {
        UserMute mute = new()
        {
            UserID = target.ID,
            Moderator = moderator.ID,
            CreatedAt = time.GetUtcNow(),
            Expiry = expiry,
            Reason = reason,
        };
        
        _mutes.Add(mute);   
        
        await using ModHelperContext context = await contextFactory.CreateDbContextAsync();
        await context.Mutes.AddAsync(mute);
        await context.SaveChangesAsync();
        
        Result muteResult = await guildApi.AddGuildMemberRoleAsync(_guildID, mute.UserID, _muteRoleID, $"{moderator.Username}: {reason}".Truncate(50, "[...]"));

        await logging.LogActionAsync(target, moderator, expiry, reason);

        return muteResult;
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