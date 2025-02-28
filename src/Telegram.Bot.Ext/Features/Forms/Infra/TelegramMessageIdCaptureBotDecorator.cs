namespace Telegram.Bot.Ext.Features.Forms.Infra;

internal sealed class TelegramMessageIdCaptureBotDecorator : ITelegramBot
{
    private readonly ITelegramBot _inner;

    public TelegramMessageIdCaptureBotDecorator(ITelegramBot inner, Action<int> onMessageIdReceived)
    {
        _inner = inner;
        Client = new TelegramMessageIdCaptureBotClientDecorator(inner.Client, onMessageIdReceived);
    }

    public ITelegramBotClient Client { get; }

    public T GetFeature<T>() where T : notnull => _inner.GetFeature<T>();
}
