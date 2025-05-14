using Telegram.Bot.Ext.Features.Users.Models;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Users.Handlers;

public class GateTelegramHandler : ITelegramHandler
{
    private readonly Group[] _allowedGroups;

    public GateTelegramHandler(IUsersProvider usersProvider, Group[] allowedGroups)
    {
        _allowedGroups = allowedGroups;
    }

    public async Task HandleAsync(HandleNext next, Update request, IHandleContext ctx, CancellationToken token)
    {
        if (await ctx.Bot.GetUsersProvider().CheckIfAsync(ctx.UserId, _allowedGroups))
            await next(ctx);
    }
}