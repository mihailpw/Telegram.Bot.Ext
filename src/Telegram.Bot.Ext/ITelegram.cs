using Microsoft.Extensions.DependencyInjection;

namespace Telegram.Bot.Ext;

public interface ITelegramManager
{
    ITelegramBotClient Bot { get; }

    T GetFeature<T>() where T : notnull;
}

public class TelegramManager : ITelegramManager
{
    private readonly IServiceProvider _serviceProvider;

    public TelegramManager(ITelegramBotClient bot, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        Bot = bot;
    }

    public ITelegramBotClient Bot { get; }

    public T GetFeature<T>() where T : notnull
    {
        return _serviceProvider.GetRequiredService<T>();
    }
}
