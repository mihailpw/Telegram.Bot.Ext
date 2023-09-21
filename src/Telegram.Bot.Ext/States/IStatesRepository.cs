using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.States;

public interface IStatesRepository
{
    Task<IState> GetOrCreateAsync(ChatId chatId, long userId);
    Task SaveAsync(IState state);
}

public class InMemoryStatesRepository : IStatesRepository
{
    private readonly Dictionary<(ChatId chatId, long userId), IState> _states = new();

    public Task<IState> GetOrCreateAsync(ChatId chatId, long userId)
    {
        var key = (chatId, userId);
        if (!_states.TryGetValue(key, out var state))
        {
            state = new InMemoryImmediateState(chatId, userId);
            _states[key] = state;
        }

        return Task.FromResult(state);
    }

    public Task SaveAsync(IState state)
    {
        // ignore
        return Task.CompletedTask;
    }
}