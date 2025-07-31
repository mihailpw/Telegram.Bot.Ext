using Telegram.Bot.Ext.Handlers.Base;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Building.Internal;

internal class CompositeTelegramHandler : TelegramHandlerBase
{
    private readonly TelegramHandlersExecutor _telegramHandlersExecutor;

    public CompositeTelegramHandler(TelegramHandlersExecutor telegramHandlersExecutor)
    {
        _telegramHandlersExecutor = telegramHandlersExecutor;
    }

    protected override async Task<bool> HandleAsync(Update request, IHandleContext ctx, CancellationToken token)
    {
        return await _telegramHandlersExecutor.ExecuteAsync(request, ctx, token);
    }
}