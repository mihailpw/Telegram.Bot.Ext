using Telegram.Bot.Ext.Core;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext;

public static partial class TelegramBotExt
{
    public static async Task<IAsyncDisposable> SendProcessingMessageAsync(
        this ITelegramBot bot,
        ChatId chatId,
        string processingText,
        CancellationToken token)
    {
        var message = await bot.Client.SendTextMessageAsync(chatId, processingText, cancellationToken: token);
        return new AsyncDisposable(() => bot.DeleteMessageAsync(chatId, message.MessageId, cancellationToken: token));
    }
}
