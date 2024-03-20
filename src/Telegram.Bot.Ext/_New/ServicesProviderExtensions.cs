namespace Telegram.Bot.Ext.Building;

public static class ServicesProviderExtensions
{
    public static IServiceProvider UseTelegramHandler(this IServiceProvider sp, Action<IHandlersSetup> configure)
    {
        //var handler = new TelegramUpdateHandler(sp.GetService<>())
        return sp;
    }
}