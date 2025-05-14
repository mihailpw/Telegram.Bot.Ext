using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Handlers.Base;

public abstract class GroupMultiCommandTelegramHandlerBase : MultiCommandTelegramHandlerBase
{
    protected void RegisterCommand(
        string command,
        Handle handler,
        Group group)
    {
        RegisterCommand(
            command,
            handler,
            (message, ctx, _) => ctx.Bot.GetUsersProvider().CheckIfAsync(message.From!.Id, group));
    }

    protected void RegisterCommand(
        string command,
        Handle handler,
        params Group[] groups)
    {
        RegisterCommand(
            command,
            handler,
            async (message, ctx, _) =>
            {
                var userGroup = await ctx.Bot.GetUsersProvider().GetGroupAsync(message.From!.Id);
                return userGroup.HasValue && groups.Contains(userGroup.Value);
            });
    }
}