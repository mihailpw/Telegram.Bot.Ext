using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.CallbackQueries;

public delegate Task<(bool handled, bool removeMarkup)> CallbackQueryHandler(
    CallbackQuery query, string? data, IHandleContext ctx, CancellationToken token);