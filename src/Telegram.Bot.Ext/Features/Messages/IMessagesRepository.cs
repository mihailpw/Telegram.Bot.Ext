using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Messages;

public interface IMessagesRepository<TMessage> where TMessage : IMessage
{
    Task<IReadOnlyCollection<TMessage>> GetAllAsync();
    IAsyncEnumerable<TMessage> GetAllAwait();
    Task<IReadOnlyCollection<TMessage>> GetAsync(string groupKey);
    Task<IReadOnlyCollection<TMessage>> GetAsync(ChatId chatId);
    Task<IReadOnlyCollection<TMessage>> GetAsync(ChatId chatId, string groupKey);
    Task<TMessage?> GetAsync(ChatId chatId, int messageId);
    Task<TMessage?> GetAsync(ChatId chatId, int messageId, string groupKey);

    Task<bool> AddAsync(TMessage item);
    Task DeleteAsync(IEnumerable<TMessage> messagesToRemove);
}

public class InMemoryMessagesRepository<TMessage> : IMessagesRepository<TMessage> where TMessage : IMessage
{
    private readonly List<TMessage> _messages = new();

    public Task<IReadOnlyCollection<TMessage>> GetAllAsync()
        => Task.FromResult<IReadOnlyCollection<TMessage>>(_messages);

    public IAsyncEnumerable<TMessage> GetAllAwait()
        => _messages.ToAsyncEnumerable();

    public Task<IReadOnlyCollection<TMessage>> GetAsync(string groupKey)
        => Task.FromResult<IReadOnlyCollection<TMessage>>(
            _messages.Where(m => m.GroupKey == groupKey).ToList());

    public Task<IReadOnlyCollection<TMessage>> GetAsync(ChatId chatId)
        => Task.FromResult<IReadOnlyCollection<TMessage>>(
            _messages.Where(m => m.ChatId == chatId).ToList());

    public Task<IReadOnlyCollection<TMessage>> GetAsync(ChatId chatId, string groupKey)
        => Task.FromResult<IReadOnlyCollection<TMessage>>(
            _messages.Where(m => m.ChatId == chatId && m.GroupKey == groupKey).ToList());

    public Task<TMessage?> GetAsync(ChatId chatId, int messageId)
        => Task.FromResult(_messages.FirstOrDefault(
                m => m.ChatId == chatId && m.MessageId == messageId));

    public Task<TMessage?> GetAsync(ChatId chatId, int messageId, string groupKey)
        => Task.FromResult(_messages.FirstOrDefault(
                m => m.ChatId == chatId && m.MessageId == messageId && m.GroupKey == groupKey));

    public Task<bool> AddAsync(TMessage item)
    {
        if (_messages.Any(m => m.ChatId == item.ChatId && m.MessageId == item.MessageId && m.GroupKey == item.GroupKey))
            return Task.FromResult(false);
        _messages.Add(item);
        return Task.FromResult(true);
    }

    public Task DeleteAsync(IEnumerable<TMessage> messagesToRemove)
    {
        foreach (var messageToRemove in messagesToRemove)
            _messages.Remove(messageToRemove);
        return Task.CompletedTask;
    }
}

public interface IMessage : IEquatable<IMessage>
{
    public ChatId ChatId { get; }
    public int MessageId { get; }
    public string GroupKey { get; }
}

public class Message : IMessage
{
    public Message(ChatId chatId, int messageId, string groupKey)
    {
        ChatId = chatId;
        MessageId = messageId;
        GroupKey = groupKey;
    }

    public ChatId ChatId { get; }
    public int MessageId { get; }
    public string GroupKey { get; }

    public bool Equals(IMessage? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return ChatId.Equals(other.ChatId) && MessageId == other.MessageId && GroupKey == other.GroupKey;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Message)obj);
    }

    public override int GetHashCode() => HashCode.Combine(ChatId, MessageId, GroupKey);
}
