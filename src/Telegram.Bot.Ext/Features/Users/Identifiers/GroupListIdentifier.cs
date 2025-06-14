using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users.Identifiers;

public class GroupListIdentifier : Identifier
{
    private readonly HashSet<Group> _groups;

    public GroupListIdentifier(IEnumerable<Group> groups)
    {
        _groups = new HashSet<Group>(groups);
    }

    public new static GroupListIdentifier Parse(string input)
        => new(input.Split(',', StringSplitOptions.TrimEntries).Select(g => new Group(g)));

    public override IAsyncEnumerable<long> PrepareChatIdsAwait(IUsersProvider usersProvider)
    {
        return _groups.ToAsyncEnumerable().SelectMany(usersProvider.GetAllAwait);
    }

    public override string ToString() => string.Join(',', _groups);
}