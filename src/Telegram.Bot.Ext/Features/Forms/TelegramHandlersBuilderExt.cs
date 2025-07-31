using Telegram.Bot.Ext.Handlers;

// ReSharper disable once CheckNamespace
namespace Telegram.Bot.Ext.Building;

public static partial class TelegramHandlersBuilderExt
{
    public static ITelegramHandlersBuilder UseForms(this ITelegramHandlersBuilder builder)
    {
        return builder.Use<FormsTelegramHandler>();
    }
}