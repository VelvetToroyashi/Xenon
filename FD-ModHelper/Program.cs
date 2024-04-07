
using FDModHelper.Data;
using FDModHelper.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables("FDMH_")
    .Build();
    
var services = new ServiceCollection();

services.AddSingleton<MuteService>();
services.AddSingleton<IConfiguration>(configuration);
services.AddDbContext<ModHelperContext>(options => options.UseSqlite(), ServiceLifetime.Transient);