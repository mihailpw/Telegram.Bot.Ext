using Microsoft.Extensions.DependencyInjection;

namespace Telegram.Bot.Ext;

public interface ITelegramBot
{
    ITelegramBotClient Client { get; }

    T GetFeature<T>() where T : notnull;
}

public class TelegramBot : ITelegramBot
{
    private readonly IServiceProvider _serviceProvider;

    public TelegramBot(ITelegramBotClient client, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        Client = client;
    }

    public ITelegramBotClient Client { get; }

    public T GetFeature<T>() where T : notnull
    {
        return _serviceProvider.GetRequiredService<T>();
    }
}
