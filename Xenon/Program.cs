
using Xenon.Data;
using Xenon.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Commands.Extensions;
using Remora.Discord.Commands.Extensions;
using RemoraDelegateDispatch.Extensions;
using Serilog;
using Xenon.Checks;
using Xenon.Commands;

IHostBuilder hostBuilder =
Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(config => config.AddEnvironmentVariables("XENON_"))
    .ConfigureLogging(l => l.ClearProviders().AddSerilog())
    .ConfigureServices
    (
        (_, services) =>
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            services.AddSingleton<MuteService>();
            services.AddHostedService(s => s.GetRequiredService<MuteService>());
            services.AddSingleton<LoggingService>();
            services.AddDbContext<ModHelperContext>(options => options.UseSqlite(), ServiceLifetime.Transient);

            services.AddDiscordGateway(s => s.GetRequiredService<IConfiguration>()["DiscordToken"]!);

            services.AddDiscordCommands(true)
                    .AddCommandTree()
                    .WithCommandGroup<ModerationCommands>();

            services.AddDelegateResponders();
        }
    );

IHost host = hostBuilder.Build();

await host.RunAsync();
