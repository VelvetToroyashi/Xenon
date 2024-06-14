using System.ComponentModel;
using Humanizer;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Contexts;
using Remora.Results;
using Xenon.Checks;
using Xenon.Services;

namespace Xenon.Commands;

[Group("vc")]
[RequiresJanitorRole]
[AllowedContexts(InteractionContextType.Guild)]
public class ModerationCommands(IInteractionContext context, IDiscordRestInteractionAPI interactions, MuteService mutes) : CommandGroup
{
    [Command("ban")]
    [Description("Banishes a user from voice channels.")]
    public async Task<IResult> BanishAsync(IUser target, TimeSpan duration, string reason)
    {
        Result muteResult = await mutes.VCBanUserAsync
        (
            target,
            context.Interaction.Member.Value.User.Value,
            DateTimeOffset.UtcNow + duration,
            reason
        );

        string message = muteResult.IsSuccess
            ? $"<@{target.ID}> has been banished from voice channels for {duration.Humanize()} due to: {reason}."
            : $"Failed to banish user <@{target.ID}> from voice channels.";

        return await interactions.CreateFollowupMessageAsync
        (
            context.Interaction.ApplicationID,
            context.Interaction.Token,
            message,
            flags: MessageFlags.Ephemeral
        );
    }

    [Command("unban")]
    [Description("Unbanishes or unquarantines a user from voice channels.")]
    public async Task<IResult> UnbanishAsync(IUser target)
    {
        Result unmuteResult = await mutes.UnVCBanUserAsync(target);

        string message = unmuteResult.IsSuccess
            ? $"<@{target.ID}> has been unbanished from voice channels."
            : $"Failed to unbanish user <@{target.ID}> from voice channels.";

        return await interactions.CreateFollowupMessageAsync
        (
            context.Interaction.ApplicationID,
            context.Interaction.Token,
            message,
            flags: MessageFlags.Ephemeral
        );
    }

}
