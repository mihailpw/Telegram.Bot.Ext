using Telegram.Bot.Ext.Core;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext;

public static partial class TelegramBotClientExt
{
    public static async Task<IAsyncDisposable> SendProcessingMessageAsync(
        this ITelegramBotClient bot,
        ChatId chatId,
        string processingText,
        CancellationToken token)
    {
        var message = await bot.SendTextMessageAsync(chatId, processingText, cancellationToken: token);
        return new AsyncDisposable(() => bot.DeleteMessageAsync(chatId, message.MessageId, cancellationToken: token));
    }
}
