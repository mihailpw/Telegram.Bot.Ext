using Telegram.Bot.Ext.Handlers.Base;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.CallbackQueries;

public sealed class CallbackQueryProcessorTelegramHandler : TelegramHandlerBase
{
    private readonly ICallbackQueryManager _callbackQueryManager;

    public CallbackQueryProcessorTelegramHandler(ICallbackQueryManager callbackQueryManager)
    {
        _callbackQueryManager = callbackQueryManager;
    }

    protected override async Task<bool> HandleAsync(Update request, IHandleContext ctx, CancellationToken token)
    {
        if (request is not { CallbackQuery.Message: { } message })
            return false;

        var (handled, removeMarkup) = await _callbackQueryManager.HandleAsync(request.CallbackQuery, ctx, token);
        if (removeMarkup)
            await ctx.Bot.EditMessageReplyMarkupAsync(ctx.ChatId, message.MessageId, null, token);

        return handled;
    }
}