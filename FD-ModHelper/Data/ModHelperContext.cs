using Microsoft.EntityFrameworkCore;

namespace FDModHelper.Data;

public class ModHelperContext : DbContext
{
    public DbSet<UserMute> Mutes { get; set; } = null!;
    
    public ModHelperContext(DbContextOptions<ModHelperContext> options) : base(options)
    {
    }
}