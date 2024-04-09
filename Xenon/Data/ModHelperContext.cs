using Microsoft.EntityFrameworkCore;

namespace Xenon.Data;

public class ModHelperContext(DbContextOptions<ModHelperContext> options) : DbContext(options)
{
    public required DbSet<UserMute> Mutes { get; set; } 
    
    public required DbSet<UserChannelChange> ChannelChanges { get; set; }
}