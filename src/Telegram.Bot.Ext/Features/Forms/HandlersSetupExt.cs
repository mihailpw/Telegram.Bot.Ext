using Telegram.Bot.Ext.Handlers;

// ReSharper disable once CheckNamespace
namespace Telegram.Bot.Ext.Building;

public static partial class HandlersSetupExt
{
    public static IHandlersSetup UseForms(this IHandlersSetup setup)
    {
        return setup.Use<FormsTelegramHandler>();
    }
}