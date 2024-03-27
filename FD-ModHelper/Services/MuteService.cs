using FDModHelper.Data;

namespace FDModHelper.Services;

public sealed class MuteService(ModHelperContext context)
{
    private readonly List<UserMute> _mutes = new();
    
    public async Task RunAsync(CancellationToken cancellationToken)
    {
    }
}