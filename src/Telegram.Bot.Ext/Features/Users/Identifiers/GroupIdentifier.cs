using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users.Identifiers;

public class GroupIdentifier : Identifier
{
    private readonly Group _group;

    public GroupIdentifier(Group group)
    {
        _group = group;
    }

    public new static GroupIdentifier Parse(string data) => new(new Group(data));

    public override IAsyncEnumerable<long> PrepareChatIdsAwait(IUsersProvider usersProvider)
    {
        return usersProvider.GetAllAwait(_group);
    }

    public override string ToString() => _group.ToString();
}