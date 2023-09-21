using Telegram.Bot.Ext.Handlers.Base;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Building.Internal;

internal class CompositeTelegramHandler : TelegramHandlerBase
{
    private readonly HandlersExecutor _handlersExecutor;

    public CompositeTelegramHandler(HandlersExecutor handlersExecutor)
    {
        _handlersExecutor = handlersExecutor;
    }

    protected override async Task<bool> HandleAsync(Update request, IHandleContext ctx, CancellationToken token)
    {
        return await _handlersExecutor.ExecuteAsync(request, ctx, token);
    }
}