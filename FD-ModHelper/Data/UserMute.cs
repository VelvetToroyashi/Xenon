using Remora.Rest.Core;

namespace FDModHelper.Data;

/// <summary>
/// Represents a mute for a user.
/// </summary>
public record UserMute(Snowflake UserID, Snowflake Moderator, DateTimeOffset Expiry, string Reason);