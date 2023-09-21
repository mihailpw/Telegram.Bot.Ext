using Telegram.Bot.Ext.Handlers.Base;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Handlers;

public class SaveTextMessagesToFileTelegramHandler : PassingTelegramHandlerBase
{
    private const string BotIdPlaceholder = "{bot_id}";

    private readonly string _filePath;
    private readonly bool _hasBotIdPlaceholder;

    public SaveTextMessagesToFileTelegramHandler(string filePath)
    {
        _filePath = filePath;
        _hasBotIdPlaceholder = filePath.Contains(BotIdPlaceholder);
    }

    protected override async Task HandleAsync(Update request, IHandleContext ctx, CancellationToken token)
    {
        if (request.Message is { Text: not null } message)
        {
            var content = $"{message.MessageId}: {message.Text}{Environment.NewLine}";
            var filePath = _hasBotIdPlaceholder
                ? _filePath.Replace(BotIdPlaceholder, ctx.Bot.BotId.ToString())
                : _filePath;

            await System.IO.File.AppendAllTextAsync(filePath, content, token);
        }
    }
}
