using Telegram.Bot.Ext.Handlers.Base;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Handlers;

public class CleanIncomingMessageTelegramHandler : PassingTelegramHandlerBase
{
    protected override async Task HandleAsync(Update request, IHandleContext ctx, CancellationToken token)
    {
        if (request.Message is { } message)
            await ctx.Bot.DeleteMessageAsync(ctx.ChatId, message.MessageId, token);
    }
}