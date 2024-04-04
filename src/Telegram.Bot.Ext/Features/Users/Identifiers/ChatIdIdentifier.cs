namespace Telegram.Bot.Ext.Features.Users.Identifiers;

public class ChatIdIdentifier : Identifier
{
    private readonly long _chatId;

    public ChatIdIdentifier(long chatId)
    {
        _chatId = chatId;
    }

    public override async IAsyncEnumerable<long> PrepareChatIdsAwait(IUsersProvider usersProvider)
    {
        yield return _chatId;
        await Task.CompletedTask;
    }

    public override string ToString() => _chatId.ToString();
}