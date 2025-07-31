namespace Telegram.Bot.Ext.Building;

public interface ITelegramHandlersBuilder
{
    ITelegramHandlersBuilder Use(Func<IServiceProvider, ITelegramHandler> telegramHandlerFactory);
}