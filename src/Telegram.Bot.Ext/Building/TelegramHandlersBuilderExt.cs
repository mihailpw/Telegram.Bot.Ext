using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Ext.Building.Internal;

namespace Telegram.Bot.Ext.Building;

public static partial class TelegramHandlersBuilderExt
{
    public static ITelegramHandlersBuilder Use(this ITelegramHandlersBuilder builder, Type type, params object[] parameters)
    {
        if (!type.IsClass)
            throw new ArgumentException($"Type '{type.Name}' is not class", nameof(type));
        if (!typeof(ITelegramHandler).IsAssignableFrom(type))
            throw new ArgumentException($"Type '{type.Name}' is not implement '{nameof(ITelegramHandler)}' interface", nameof(type));

        return builder.Use(sp => (ITelegramHandler)ActivatorUtilities.CreateInstance(sp, type, parameters));
    }

    public static ITelegramHandlersBuilder Use<T>(this ITelegramHandlersBuilder builder, params object[] parameters)
        where T : class, ITelegramHandler
    {
        return builder.Use(sp => ActivatorUtilities.CreateInstance<T>(sp, parameters));
    }

    public static ITelegramHandlersBuilder UseMulti(this ITelegramHandlersBuilder builder, Action<ITelegramHandlersBuilder> setupAction)
    {
        var executorBuilder = new TelegramHandlersExecutorBuilder();
        setupAction(builder);
        return builder.Use(sp => new CompositeTelegramHandler(executorBuilder.Build(sp)));
    }

    public static ITelegramHandlersBuilder UseAutoRegistered(this ITelegramHandlersBuilder builder, Assembly assemblyToScan)
    {
        foreach (var type in assemblyToScan.GetTypes())
        {
            if (type.GetCustomAttribute<AutoRegisteredTelegramCommandAttribute>()
                is { Disabled: false } attr)
                builder.Use(type, attr.Parameters);
        }

        return builder;
    }
}