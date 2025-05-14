using Telegram.Bot.Ext.Core;
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

public abstract class GroupTelegramHandlerBase : ITelegramHandler
{
    private readonly Group[] _allowedGroups;

    protected GroupTelegramHandlerBase(params Group[] allowedGroups)
    {
        _allowedGroups = allowedGroups;
    }

    public async Task HandleAsync(HandleNext next, Update request, IHandleContext ctx, CancellationToken token)
    {
        if (await ctx.Bot.GetUsersProvider().CheckIfAsync(ctx.UserId, _allowedGroups)
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
