using Microsoft.Extensions.Configuration;
using Remora.Commands.Conditions;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
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
        var janitorRoles = configuration.GetSection("JanitorRoleIDs").Get<ulong[]>()!.Select(c => new Snowflake(c));
        var hasPermission = context.Interaction.Member.Value.Roles.Intersect(janitorRoles).Any();

        if (hasPermission)
        {
            return Result.Success;
        }

        await interactions.CreateInteractionResponseAsync
        (
            context.Interaction.ID,
            context.Interaction.Token,
            new InteractionResponse
            (
                InteractionCallbackType.ChannelMessageWithSource,
                new(new InteractionMessageCallbackData
                {
                    Content = "You do not have permission to use this command.",
                    Flags = MessageFlags.Ephemeral,
                })
            ),
            ct: ct
        );

        return Result.FromError(new PermissionDeniedError("User does not have permission to use this command."));

    }
}
