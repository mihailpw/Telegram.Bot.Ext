using Telegram.Bot.Ext.Core;
using Telegram.Bot.Ext.Features.Users;
using Telegram.Bot.Ext.Features.Users.Models;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Handlers.Base;

public abstract class TelegramHandlerBase : ITelegramHandler
{
    public async Task HandleAsync(HandleNext next, Update request, IHandleContext ctx, CancellationToken token)
    {
        if (!await HandleAsync(request, ctx, token))
            await next(ctx);
    }

    protected abstract Task<bool> HandleAsync(Update request, IHandleContext ctx, CancellationToken token);
}

public abstract class RoleTelegramHandlerBase : ITelegramHandler
{
    private readonly IUsersProvider _usersProvider;
    private readonly Role[] _allowedRoles;

    protected RoleTelegramHandlerBase(IUsersProvider usersProvider, params Role[] allowedRoles)
    {
        _usersProvider = usersProvider;
        _allowedRoles = allowedRoles;
    }

    public async Task HandleAsync(HandleNext next, Update request, IHandleContext ctx, CancellationToken token)
    {
        if (await _usersProvider.CheckIfAsync(ctx.UserId, _allowedRoles)
            && await HandleAsync(request, ctx, token))
            return;

        await next(ctx);
    }

    protected abstract Task<bool> HandleAsync(Update request, IHandleContext ctx, CancellationToken token);
}

public abstract class PassingTelegramHandlerBase : ITelegramHandler
{
    public async Task HandleAsync(HandleNext next, Update request, IHandleContext ctx, CancellationToken token)
    {
        HandleAsync(request, ctx, token).Forget();
        await next(ctx);
    }

    protected abstract Task HandleAsync(Update request, IHandleContext ctx, CancellationToken token);
}
