using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Handlers.Routing;

public class RouterTelegramHandler : ITelegramHandler
{
    public delegate Task<bool> AsyncSelector(Update request, IHandleContext ctx, CancellationToken token);

    private readonly IReadOnlyList<(AsyncSelector selector, ITelegramHandler handler)> _handlers;
    private readonly ITelegramHandler _defaultHandler;

    public RouterTelegramHandler(IReadOnlyList<(AsyncSelector selector, ITelegramHandler handler)> handlers,
        ITelegramHandler defaultHandler)
    {
        _handlers = handlers;
        _defaultHandler = defaultHandler;
    }

    public async Task HandleAsync(HandleNext next, Update request, IHandleContext ctx, CancellationToken token)
    {
        foreach (var (selector, handler) in _handlers)
        {
            if (await selector(request, ctx, token))
            {
                await handler.HandleAsync(next, request, ctx, token);
                return;
            }
        }

        await _defaultHandler.HandleAsync(next, request, ctx, token);
    }
}