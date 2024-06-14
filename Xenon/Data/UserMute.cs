using Remora.Rest.Core;

namespace Xenon.Data;

/// <summary>
/// Represents a mute for a user.
/// </summary>
public class UserMute
{
    /// <summary>
    /// The ID of the mute. Purely used for database purposes.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// The ID of the user who received the mute.
    /// </summary>
    public required Snowflake UserID { get; init; }

    /// <summary>
    /// The ID of the moderator who issued the mute.
    /// </summary>
    public required Snowflake Moderator { get; init; }

    /// <summary>
    /// When the mute was created.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// When the mute expires.
    /// </summary>
    public required DateTimeOffset Expiry { get; init; }

    public required Snowflake AppliedRoleID { get; init; }

    /// <summary>
    /// The reason for the mute.
    /// </summary>
    public required string Reason { get; init; }
}
