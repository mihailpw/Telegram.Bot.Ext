using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Telegram.Bot.Ext.Building.Internal;

internal class TelegramHandlersExecutorBuilder : ITelegramHandlersBuilder
{
    private readonly List<Func<IServiceProvider, ITelegramHandler>> _handlerFactories = new();

    public ITelegramHandlersBuilder Use(Func<IServiceProvider, ITelegramHandler> telegramHandlerFactory)
    {
        _handlerFactories.Add(telegramHandlerFactory);
        return this;
    }

    public TelegramHandlersExecutor Build(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetService<ILogger<TelegramHandlersExecutor>>()
            ?? NullLogger<TelegramHandlersExecutor>.Instance;
        var handlers = _handlerFactories.Select(hf => hf(serviceProvider)).ToList();

        return new TelegramHandlersExecutor(logger, handlers);
    }
}