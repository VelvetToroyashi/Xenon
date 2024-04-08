
using Xenon.Data;
using Xenon.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

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
            services.AddSingleton<LoggingService>();
            services.AddDbContext<ModHelperContext>(options => options.UseSqlite(), ServiceLifetime.Transient);
        }
    );
    
IHost host = hostBuilder.Build();

await host.RunAsync();