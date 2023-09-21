using Telegram.Bot.Ext.Features.Users;
using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Handlers.Base;

public abstract class RoleMultiCommandTelegramHandlerBase : MultiCommandTelegramHandlerBase
{
    private readonly IUsersProvider _usersProvider;

    protected RoleMultiCommandTelegramHandlerBase(IUsersProvider usersProvider)
    {
        _usersProvider = usersProvider;
    }

    protected void RegisterCommand(
        string command,
        Handle handler,
        Role role)
    {
        RegisterCommand(
            command,
            handler,
            (message, _, _) => _usersProvider.CheckIfAsync(message.From!.Id, role));
    }
}