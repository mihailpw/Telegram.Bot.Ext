using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Telegram.Bot.Ext.Building.Internal;
using Telegram.Bot.Ext.States;
using Telegram.Bot.Polling;

namespace Telegram.Bot.Ext.Building;

public class TelegramUpdateHandlerBuilder
{
    private readonly HandlersExecutorBuilder _handlersExecutorBuilder = new();
    private Func<IServiceProvider, IStatesRepository>? _statesRepositoryFactory;

    public TelegramUpdateHandlerBuilder SetupStatesRepository(
        Func<IServiceProvider, IStatesRepository> statesRepositoryFactory)
    {
        _statesRepositoryFactory = statesRepositoryFactory;
        return this;
    }

    public TelegramUpdateHandlerBuilder SetupHandlers(Action<IHandlersSetup> setupAction)
    {
        setupAction(_handlersExecutorBuilder);
        return this;
    }

    public IUpdateHandler Build(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetService<ILogger<TelegramUpdateHandler>>()
            ?? NullLogger<TelegramUpdateHandler>.Instance;
        var statesRepository = _statesRepositoryFactory?.Invoke(serviceProvider)
            ?? serviceProvider.GetService<IStatesRepository>()
            ?? new InMemoryStatesRepository();

        return new TelegramUpdateHandler(logger, statesRepository, _handlersExecutorBuilder.Build(serviceProvider));
    }
}