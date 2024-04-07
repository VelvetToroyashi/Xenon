using FDModHelper.Data;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Gateway;
using Remora.Rest.Core;
using Remora.Results;

namespace FDModHelper.Services;

public sealed class MuteService
(
    IConfiguration config,
    LoggingService logging,
    DiscordGatewayClient gateway,
    IDiscordRestGuildAPI guildApi,
    IDbContextFactory<ModHelperContext> contextFactory
)
{
    private readonly List<UserMute> _mutes = new();
    
    private Task _gatewayTask = Task.CompletedTask;
    
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        _gatewayTask = RunGatewayAsync(cancellationToken);
        
        await Task.Delay(-1, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
    }
    
    public async Task<Result> AddMuteAsync(IUser target, IUser moderator, DateTimeOffset expiry, string reason)
    {
        var mute = new UserMute(target.ID, moderator.ID, expiry, reason);
        
        _mutes.Add(mute);   
        
        await using var context = await contextFactory.CreateDbContextAsync();
        await context.Mutes.AddAsync(mute);
        await context.SaveChangesAsync();
        
        var muteRoleID = config.GetValue<ulong>("XENON_MuteRoleID");
        var guildID = config.GetValue<ulong>("XENON_DiscordGuildID");
        
        var muteResult = await guildApi.AddGuildMemberRoleAsync(new(guildID), mute.UserID, new(muteRoleID), mute.Reason.Truncate(50, "[...]"));

        await logging.LogActionAsync(new(guildID), target, moderator, expiry, reason);

        return muteResult;
    }
    
    private async Task RunGatewayAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            var gatewayTaskResult = await gateway.RunAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

            if (gatewayTaskResult.IsSuccess)
            {
                return;
            }
        }
    }
}