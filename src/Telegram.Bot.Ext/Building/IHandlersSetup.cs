namespace Telegram.Bot.Ext.Building;

public interface IHandlersSetup
{
    IHandlersSetup Use(Func<IServiceProvider, ITelegramHandler> telegramHandlerFactory);
}