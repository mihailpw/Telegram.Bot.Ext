using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.CallbackQueries;

public interface ICallbackQueryManager
{
    Task<(bool handled, bool removeMarkup)> HandleAsync(CallbackQuery callbackQuery, IHandleContext ctx,
        CancellationToken token);

    string AddForHandling(CallbackQueryHandler handler, [CallerFilePath] string sourceFilePath = "",
        [CallerMemberName] string memberName = "");
}

public delegate Task<(bool handled, bool removeMarkup, bool detachSelf)> CallbackQueryHandler(
    CallbackQuery query, IHandleContext ctx, CancellationToken token);

public class CallbackQueryManager : ICallbackQueryManager
{
    private readonly ILogger<CallbackQueryManager> _logger;

    private readonly Dictionary<string, (string source, CallbackQueryHandler handler)> _handlers = new();

    public CallbackQueryManager(ILogger<CallbackQueryManager> logger)
    {
        _logger = logger;
    }

    public string AddForHandling(CallbackQueryHandler handler, [CallerFilePath] string sourceFilePath = "",
        [CallerMemberName] string memberName = "")
    {
        var source = $"{sourceFilePath}:{memberName}";
        var callbackData = $"{Guid.NewGuid():N}:{source.GetHashCode()}";
        _handlers.Add(callbackData, (source, handler));
        _logger.LogInformation("Added callback query {CallbackData} (source={Source})", callbackData, source);
        return callbackData;
    }

    public async Task<(bool handled, bool removeMarkup)> HandleAsync(CallbackQuery callbackQuery, IHandleContext ctx, CancellationToken token)
    {
        var key = callbackQuery.Data;
        if (key is null || !_handlers.TryGetValue(key, out var handlingData))
            return (false, false);

        _logger.LogInformation("Handling callback query {CallbackData} (source={Source})", key, handlingData.source);
        var handleResult = await handlingData.handler(callbackQuery, ctx, token);
        if (handleResult.detachSelf)
            _handlers.Remove(key);

        return (handleResult.handled, handleResult.removeMarkup);
    }
}