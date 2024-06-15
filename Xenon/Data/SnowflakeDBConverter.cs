using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Remora.Discord.API;
using Remora.Rest.Core;

namespace Xenon.Data;

public class SnowflakeDBConverter()
    : ValueConverter<Snowflake, ulong>(x => x.Value, x => DiscordSnowflake.New(x));

public class NullableSnowflakeDBConverter()
    : ValueConverter<Snowflake?, ulong?>
    (
        x => x.HasValue ? x.Value.Value : null,
        x => x.HasValue ? DiscordSnowflake.New(x.Value) : null
    );
