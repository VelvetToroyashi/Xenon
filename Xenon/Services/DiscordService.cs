using Microsoft.Extensions.Hosting;
using Remora.Discord.Gateway;
using Remora.Results;

namespace Xenon.Services;

public class DiscordService(DiscordGatewayClient gateway) : BackgroundService
{
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Task<Result> gatewayTask = gateway.RunAsync(stoppingToken);
            await ((Task)gatewayTask).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

            if (gatewayTask.Result.IsSuccess)
            {
                return;
            }
        }
    }
}
