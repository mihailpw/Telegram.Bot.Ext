using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.States;

public interface IState
{
    ChatId ChatId { get; }
    long UserId { get; }

    IReadOnlyCollection<string> GetKeys();
    bool Has<T>(string key);
    bool TryGet<T>(string key, out T? value);
    void Set<T>(string key, T value);
    void Remove<T>(string key);
    IState Group(string groupKey);
}

public static class StateUtils
{
    private const string KeySeparator = "__";

    public static string BuildKey(string groupKey, string key)
        => $"{groupKey}{KeySeparator}{key}";

    public static string BuildKey(params string[] keys)
        => string.Join(KeySeparator, keys);
}

public static class StateExt
{
    public static T? Get<T>(this IState state, string key)
    {
        return state.TryGet<T>(key, out var value)
            ? value
            : throw new ArgumentOutOfRangeException(nameof(key), key, "Value does not exist");
    }

    public static T? GetOrCreate<T>(this IState state, string key, T? defaultValue)
    {
        if (!state.TryGet<T>(key, out var value))
        {
            value = defaultValue;
            state.Set(key, value);
        }

        return value;
    }

    public static T? GetOrCreate<T>(this IState state, string key, Func<T> factory)
    {
        if (!state.TryGet<T>(key, out var value))
        {
            value = factory();
            state.Set(key, value);
        }

        return value;
    }
}

public class InMemoryImmediateState : IState
{
    private readonly Dictionary<string, object?> _values = new();

    public InMemoryImmediateState(ChatId chatId, long userId)
    {
        ChatId = chatId;
        UserId = userId;
    }

    public ChatId ChatId { get; }
    public long UserId { get; }

    public IReadOnlyCollection<string> GetKeys()
        => _values.Keys;

    public bool Has<T>(string key)
    {
        if (!_values.ContainsKey(key))
            return false;
        if (_values[key] is null)
            return true;

        return _values[key] is T;
    }

    public bool TryGet<T>(string key, out T? value)
    {
        if (Has<T>(key))
        {
            value = (T) _values[key]!;
            return true;
        }

        value = default;
        return false;
    }

    public void Set<T>(string key, T value)
    {
        _values[key] = value;
    }

    public void Remove<T>(string key)
    {
        _values.Remove(key);
    }

    public IState Group(string groupKey)
    {
        return new GroupStateDecorator(this, groupKey);
    }
}

public class GroupStateDecorator : IState
{
    private readonly IState _inner;
    private readonly string _groupKey;

    public GroupStateDecorator(IState inner, string groupKey)
    {
        _inner = inner;
        _groupKey = groupKey;
    }

    public ChatId ChatId => _inner.ChatId;
    public long UserId => _inner.UserId;

    public IReadOnlyCollection<string> GetKeys()
        => _inner.GetKeys()
            .Where(k => k.StartsWith(_groupKey))
            .Select(k => k[_groupKey.Length..])
            .ToList();

    public bool Has<T>(string key)
        => _inner.Has<T>(StateUtils.BuildKey(_groupKey, key));

    public bool TryGet<T>(string key, out T? value)
        => _inner.TryGet(StateUtils.BuildKey(_groupKey, key), out value);

    public void Set<T>(string key, T value)
        => _inner.Set(StateUtils.BuildKey(_groupKey, key), value);

    public void Remove<T>(string key)
        => _inner.Remove<T>(StateUtils.BuildKey(_groupKey, key));

    public IState Group(string groupKey)
        => _inner.Group(StateUtils.BuildKey(_groupKey, groupKey));
}
