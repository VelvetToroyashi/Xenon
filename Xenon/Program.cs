
using Xenon.Data;
using Xenon.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Commands.Extensions;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Discord.Gateway.Extensions;
using Remora.Results;
using RemoraDelegateDispatch.Extensions;
using Serilog;
using Xenon.Checks;
using Xenon.Commands;

IHostBuilder hostBuilder =
Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(config => config.AddEnvironmentVariables("XENON_").AddJsonFile("appsettings.json", true, true))
    .ConfigureLogging(l => l.ClearProviders().AddSerilog())
    .ConfigureServices
    (
        (_, services) =>
        {
            Log.Logger = new LoggerConfiguration()
                         .WriteTo.Console()
                         .CreateLogger();

            services.AddSingleton(TimeProvider.System);

            services.AddSingleton<MuteService>();
            services.AddHostedService(s => s.GetRequiredService<MuteService>());
            services.AddSingleton<LoggingService>();
            services.AddDbContextFactory<ModHelperContext>(options => options.UseSqlite(), ServiceLifetime.Transient);

            services.AddDiscordGateway(s => s.GetRequiredService<IConfiguration>()["DiscordToken"]!);

            services.AddDiscordCommands(true)
                    .AddCommandTree()
                    .WithCommandGroup<ModerationCommands>()
                    .Finish()
                    .AddParser<MicroTimeSpanParser>()
                    .AddCondition<RequiresJanitorRoleCheck>();

            services.AddDelegateResponders();
        }
    );

IHost host = hostBuilder.Build();

var slashService = host.Services.GetRequiredService<SlashService>();

Result updateResult = await slashService.UpdateSlashCommandsAsync();

if (!updateResult.IsSuccess)
{
    Log.Logger.Error("Failed to update slash commands: {Error}", updateResult.Error.Message);
    return;
}

await host.RunAsync();
