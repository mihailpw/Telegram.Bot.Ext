namespace Telegram.Bot.Ext.Features.Users.Identifiers;

public class ChatIdIdentifier : Identifier
{
    private readonly HashSet<long> _chatIds;

    public ChatIdIdentifier(IEnumerable<long> chatIds)
    {
        _chatIds = new HashSet<long>(chatIds);
    }

    public new static ChatIdIdentifier Parse(string input)
        => new(input.Split(',', StringSplitOptions.TrimEntries)
            .Select(d => long.TryParse(d.Trim(), out var chatId) ? chatId : (long?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value));

    public override IAsyncEnumerable<long> PrepareChatIdsAwait(IUsersProvider usersProvider)
    {
        return _chatIds.ToAsyncEnumerable();
    }

    public override string ToString() => string.Join(',', _chatIds);
}