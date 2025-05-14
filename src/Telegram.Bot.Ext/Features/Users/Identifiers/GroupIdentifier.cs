using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users.Identifiers;

public class GroupIdentifier : Identifier
{
    private readonly Group _group;

    public GroupIdentifier(Group group)
    {
        _group = group;
    }

    public override IAsyncEnumerable<long> PrepareChatIdsAwait(IUsersProvider usersProvider)
        => usersProvider.GetAllAwait(_group);

    public override string ToString() => _group.ToString();
}