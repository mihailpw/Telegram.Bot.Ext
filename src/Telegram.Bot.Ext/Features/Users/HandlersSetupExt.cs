using Telegram.Bot.Ext.Building;
using Telegram.Bot.Ext.Features.Users.Handlers;
using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users;

public static class HandlersSetupExt
{
    public static IHandlersSetup UseDefaultSubscribing(this IHandlersSetup setup)
        => setup.Use<SubscriptionTelegramHandler>();

    public static IHandlersSetup UseGate(this IHandlersSetup setup, params Role[] allowedRoles)
        => setup.Use<GateTelegramHandler>(allowedRoles);
}