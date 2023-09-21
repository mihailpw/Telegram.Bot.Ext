using Telegram.Bot.Ext.Features.Users.Models;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Users.Handlers;

public class GateTelegramHandler : ITelegramHandler
{
    private readonly IUsersProvider _usersProvider;
    private readonly Role[] _allowedRoles;

    public GateTelegramHandler(IUsersProvider usersProvider, Role[] allowedRoles)
    {
        _usersProvider = usersProvider;
        _allowedRoles = allowedRoles;
    }

    public async Task HandleAsync(HandleNext next, Update request, IHandleContext ctx, CancellationToken token)
    {
        if (await _usersProvider.CheckIfAsync(ctx.UserId, _allowedRoles))
            await next(ctx);
    }
}