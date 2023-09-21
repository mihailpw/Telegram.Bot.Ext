using Telegram.Bot.Ext.Handlers.Base;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Handlers;

public class CallbackQueryMessageRemoveMarkupTelegramHandler : TelegramHandlerBase
{
    protected override async Task<bool> HandleAsync(Update request, IHandleContext ctx, CancellationToken token)
    {
        if (request is not { CallbackQuery.Message: { } message })
            return false;

        await ctx.Bot.EditMessageReplyMarkupAsync(ctx.ChatId, message.MessageId, null, token);
        return true;
    }
}