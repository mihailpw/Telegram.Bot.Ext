using Telegram.Bot.Ext.States;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext;

public interface ITelegramHandler
{
    Task HandleAsync(HandleNext next, Update request, IHandleContext ctx, CancellationToken token);
}

public delegate Task HandleNext(IHandleContext ctx);

public interface IHandleContext
{
    ChatId ChatId { get; }
    long UserId { get; }

    IState State { get; }
    ITelegramBotClient Bot { get; }
}

internal sealed class HandleContext : IHandleContext
{
    public HandleContext(IState state, ITelegramBotClient bot)
    {
        Bot = bot;
        State = state;
    }

    public ChatId ChatId => State.ChatId;
    public long UserId => State.UserId;
    
    public IState State { get; }
    public ITelegramBotClient Bot { get; }
}
