using Telegram.Bot.Ext.Features.Messages;
using Telegram.Bot.Ext.Handlers.Base;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Handlers;

public class CallbackQueryMessageRemoveMessageTelegramHandler : TelegramHandlerBase
{
    protected override async Task<bool> HandleAsync(Update request, IHandleContext ctx, CancellationToken token)
    {
        if (request is not { CallbackQuery.Message: { } message })
            return false;

        await ctx.Bot.EditMessageTextAsync(ctx.ChatId, message.MessageId, "🤪", cancellationToken: token);
        await ctx.Bot.GetMessageRemover().ScheduleForRemovalAsync(ctx.ChatId, message.MessageId, TimeSpan.FromSeconds(10), token: token);
        return true;
    }
}