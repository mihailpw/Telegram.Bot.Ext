using Telegram.Bot.Ext.Building;

namespace Telegram.Bot.Ext.Handlers.Routing;

public class RouterTelegramHandlerBuilder : IRoutesSetup
{
    private class CompositeHandlerBuilder : IHandlersSetup
    {
        public IHandlersSetup Use(Func<IServiceProvider, ITelegramHandler> telegramHandlerFactory)
        {
            throw new NotImplementedException();
        }

        public ITelegramHandler Build()
        {
            throw new NotImplementedException();
        }
    }

    private readonly List<(RouterTelegramHandler.AsyncSelector selector, ITelegramHandler handler)> _handlers = new();
    private ITelegramHandler? _defaultHandler;

    public IRoutesSetup Redirect(RouterTelegramHandler.AsyncSelector selector, Action<IHandlersSetup> building)
    {
        var builder = new CompositeHandlerBuilder();
        building(builder);
        var handler = builder.Build();
        return this;
    }

    public IRoutesSetup Default(ITelegramHandler handler)
    {
        _defaultHandler = handler;
        return this;
    }
}