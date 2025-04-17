using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.CallbackQueries;

public class CallbackQueryManager : ICallbackQueryManager
{
    private const char Separator = ':';

    private readonly ILogger<CallbackQueryManager> _logger;

    private readonly Dictionary<Guid, (string source, CallbackQueryHandler handler)> _handlers = new();

    public CallbackQueryManager(ILogger<CallbackQueryManager> logger)
    {
        _logger = logger;
    }

    public CallbackDataId RegisterHandler(CallbackQueryHandler handler, Guid? id = null,
        [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
    {
        id ??= Guid.NewGuid();
        var key = id.Value;

        var source = $"{sourceFilePath}:{memberName}";
        _handlers.Add(key, (source, handler));
        _logger.LogInformation("Added callback query (id={Id}; source={Source})", key.ToString("N"), source);

        return new CallbackDataId(key, BuildCallbackData, _handlers.Remove);
    }

    public async Task<(bool handled, bool removeMarkup)> HandleAsync(CallbackQuery callbackQuery, IHandleContext ctx, CancellationToken token)
    {
        if (!TryParseCallbackData(callbackQuery.Data, out var id, out var data))
        {
            _logger.LogError("Bad callback query (callback={CallbackData})", callbackQuery.Data);
            return (handled: false, removeMarkup: false);
        }

        if (!_handlers.TryGetValue(id, out var handlingData))
        {
            _logger.LogWarning("Can not handle callback query (callback={CallbackData})", callbackQuery.Data);
            return (handled: false, removeMarkup: false);
        }

        _logger.LogInformation("Handling callback query (id={Id}; data={Data}; source={Source})", id, data, handlingData.source);
        return await handlingData.handler(callbackQuery, data, ctx, token);
    }

    private static string BuildCallbackData(Guid id, string? data)
    {
        return !string.IsNullOrWhiteSpace(data) ? $"{id:N}{Separator}{data}" : $"{id:N}";
    }

    private static bool TryParseCallbackData(string? callbackData, out Guid id, out string? data)
    {
        id = Guid.Empty;
        data = null;
        if (string.IsNullOrWhiteSpace(callbackData))
            return false;

        var separatorInd = callbackData.IndexOf(':');
        var idStr = separatorInd > 0 ? callbackData[..separatorInd] : callbackData;
        if (!Guid.TryParse(idStr, out id))
            return false;

        data = separatorInd > 0 ? callbackData[(separatorInd+1)..] : null;
        return true;
    }
}