using Microsoft.EntityFrameworkCore;
using Remora.Rest.Core;

namespace Xenon.Data;

public class ModHelperContext(DbContextOptions<ModHelperContext> options) : DbContext(options)
{
    public required DbSet<UserMute> Mutes { get; set; }

    public required DbSet<UserChannelChange> ChannelChanges { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Snowflake>().HaveConversion<SnowflakeDBConverter>();
        configurationBuilder.Properties<Snowflake?>().HaveConversion<NullableSnowflakeDBConverter>();

        base.ConfigureConventions(configurationBuilder);
    }
}
