using Telegram.Bot.Ext.Features.Users;
using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Handlers.Base;

public abstract class RoleMultiCommandTelegramHandlerBase : MultiCommandTelegramHandlerBase
{
    protected void RegisterCommand(
        string command,
        Handle handler,
        Role role)
    {
        RegisterCommand(
            command,
            handler,
            (message, ctx, _) => ctx.Bot.GetFeature<IUsersProvider>().CheckIfAsync(message.From!.Id, role));
    }
}