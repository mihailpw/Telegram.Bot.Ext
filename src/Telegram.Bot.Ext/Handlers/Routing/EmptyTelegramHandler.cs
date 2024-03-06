using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Handlers.Routing;

public class EmptyTelegramHandler : ITelegramHandler
{
    public static readonly ITelegramHandler Default = new EmptyTelegramHandler();

    private EmptyTelegramHandler()
    {
    }

    public Task HandleAsync(HandleNext next, Update request, IHandleContext ctx, CancellationToken token)
        => next(ctx);
}