using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Telegram.Bot.Ext.Building.Internal;

internal class HandlersExecutorBuilder : IHandlersSetup
{
    private readonly List<Func<IServiceProvider, ITelegramHandler>> _handlerFactories = new();

    public IHandlersSetup Use(Func<IServiceProvider, ITelegramHandler> telegramHandlerFactory)
    {
        _handlerFactories.Add(telegramHandlerFactory);
        return this;
    }

    public HandlersExecutor Build(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetService<ILogger<HandlersExecutor>>()
            ?? NullLogger<HandlersExecutor>.Instance;
        var handlers = _handlerFactories.Select(hf => hf(serviceProvider)).ToList();

        return new HandlersExecutor(logger, handlers);
    }
}