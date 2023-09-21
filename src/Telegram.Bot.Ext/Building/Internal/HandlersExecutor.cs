using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Building.Internal;

internal class HandlersExecutor
{
    private readonly ILogger<HandlersExecutor> _logger;
    private readonly IReadOnlyList<ITelegramHandler> _handlers;

    public HandlersExecutor(
        ILogger<HandlersExecutor> logger,
        IReadOnlyList<ITelegramHandler> handlers)
    {
        _logger = logger;
        _handlers = handlers;
    }

    public async Task<bool> ExecuteAsync(Update update, IHandleContext rootCtx, CancellationToken token)
    {
        var reachedEnd = false;

        async Task ExecuteMiddleware(IHandleContext ctx, int i)
        {
            if (i == _handlers.Count)
            {
                reachedEnd = true;
                return;
            }

            await _handlers[i].HandleAsync(
                nextCtx => ExecuteMiddleware(nextCtx, i + 1),
                update, ctx, token);
        }

        try
        {
            await ExecuteMiddleware(rootCtx, 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handling error");
        }

        return reachedEnd;
    }
}