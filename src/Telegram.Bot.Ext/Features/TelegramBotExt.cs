using Telegram.Bot.Ext.Features.CallbackQueries;
using Telegram.Bot.Ext.Features.Messages;
using Telegram.Bot.Ext.Features.Users;

// ReSharper disable once CheckNamespace
namespace Telegram.Bot.Ext;

public static partial class TelegramBotExt
{
    public static IMessageRemover GetMessageRemover(this ITelegramBot bot) => bot.GetFeature<IMessageRemover>();
    public static IUsersProvider GetUsersProvider(this ITelegramBot bot) => bot.GetFeature<IUsersProvider>();
    public static ICallbackQueryManager GetCallbackQueryManager(this ITelegramBot bot) => bot.GetFeature<ICallbackQueryManager>();
}
