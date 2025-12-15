using System.Runtime.CompilerServices;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.CallbackQueries;

public interface ICallbackQueryManager
{
    CallbackDataId RegisterHandler(CallbackQueryHandler handler, string? id = null,
        [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "");

    Task<(bool handled, bool removeMarkup)> HandleAsync(CallbackQuery callbackQuery, IHandleContext ctx,
        CancellationToken token);
}