using Remora.Rest.Core;

namespace Xenon.Data;

public class UserChannelChange
{
    public int Id { get; set; }
    
    public required Snowflake UserID { get; init; }
    
    public required Snowflake? JoinedChannelID { get; init; }
    
    public required Snowflake? LeftChannelID { get; init; }
    
    public required DateTimeOffset JoinOrLeaveDate { get; init; }
}