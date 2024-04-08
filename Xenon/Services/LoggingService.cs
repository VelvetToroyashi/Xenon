using Microsoft.Extensions.Configuration;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Rest.Core;

namespace Xenon.Services;

public class LoggingService(IConfiguration config, IDiscordRestChannelAPI channels)
{
    public async Task LogActionAsync(IUser user, IUser moderator, DateTimeOffset expiration, string reason = "not specified")
    {
        var logChannelID = config.GetValue<ulong>("XENON_LoggingChannelID");
        
        var expirationTimestamp = expiration.ToUnixTimeSeconds();
        
        // TODO: Change with display name, but Remora annoyingly removed this because Discord had 
        // inaccurately documented its optionality.
        var embed = new Embed
        (
            Title: $"{moderator.Username} muted {user.Username}",
            Description: $"Reason: {reason}",
            Fields: new([
                new EmbedField("Target ID", user.ID.ToString(), true),
                new EmbedField("Moderator ID", moderator.ID.ToString(), true),
                new EmbedField("Expiration", $"<t:{expirationTimestamp}:R>\n(<t:{expirationTimestamp}:t>)", true)
            ])
        );

        await channels.CreateMessageAsync(new(logChannelID), embeds: new([embed]));
    }
}