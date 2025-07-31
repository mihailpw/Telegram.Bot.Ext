using Telegram.Bot.Ext.Building;
using Telegram.Bot.Ext.Features.Users.Handlers;
using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users;

public static class HandlersSetupExt
{
    public static ITelegramHandlersBuilder UseDefaultSubscribing(this ITelegramHandlersBuilder builder)
        => builder.Use<SubscriptionTelegramHandler>();

    public static ITelegramHandlersBuilder UseGate(this ITelegramHandlersBuilder builder, params Group[] allowedGroups)
        => builder.Use<GateTelegramHandler>(allowedGroups);
}