using Microsoft.Extensions.Configuration;
using Remora.Commands.Conditions;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Results;
using Remora.Rest.Core;
using Remora.Results;

namespace Xenon.Checks;

public class RequiresJanitorRoleAttribute : ConditionAttribute;

public class RequiresJanitorRoleCheck
(
    IConfiguration configuration,
    IDiscordRestInteractionAPI interactions,
    IInteractionContext context
) : ICondition<RequiresJanitorRoleAttribute>
{

    public async ValueTask<Result> CheckAsync
    (
        RequiresJanitorRoleAttribute _,
        CancellationToken ct = default
    )
    {
        var hasPermission = context.Interaction.Member.Value.Roles.Contains(new Snowflake(configuration.GetValue<ulong>("JanitorRoleID")));

        if (hasPermission)
        {
            return Result.Success;
        }

        await interactions.CreateFollowupMessageAsync
        (
            context.Interaction.ApplicationID,
            context.Interaction.Token,
            "You do not have permission to use this command.",
            flags: MessageFlags.Ephemeral,
            ct: ct
        );

        return Result.FromError(new PermissionDeniedError("User does not have permission to use this command."));

    }
}
