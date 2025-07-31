using Telegram.Bot.Ext.Building;

namespace Telegram.Bot.Ext.Handlers.Routing;

public interface IRoutesSetup
{
    IRoutesSetup Redirect(RouterTelegramHandler.AsyncSelector selector, Action<ITelegramHandlersBuilder> builder);
}