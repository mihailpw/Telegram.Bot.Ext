using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Ext.Building.Internal;

namespace Telegram.Bot.Ext.Building;

public static partial class HandlersSetupExt
{
    public static IHandlersSetup Use(this IHandlersSetup setup, Type type, params object[] parameters)
    {
        if (!type.IsClass)
            throw new ArgumentException($"Type '{type.Name}' is not class", nameof(type));
        if (!typeof(ITelegramHandler).IsAssignableFrom(type))
            throw new ArgumentException($"Type '{type.Name}' is not implement '{nameof(ITelegramHandler)}' interface", nameof(type));

        return setup.Use(sp => (ITelegramHandler)ActivatorUtilities.CreateInstance(sp, type, parameters));
    }

    public static IHandlersSetup Use<T>(this IHandlersSetup setup, params object[] parameters)
        where T : class, ITelegramHandler
    {
        return setup.Use(sp => ActivatorUtilities.CreateInstance<T>(sp, parameters));
    }

    public static IHandlersSetup UseMulti(this IHandlersSetup setup, Action<IHandlersSetup> setupAction)
    {
        var builder = new HandlersExecutorBuilder();
        setupAction(builder);
        return setup.Use(sp => new CompositeTelegramHandler(builder.Build(sp)));
    }
}