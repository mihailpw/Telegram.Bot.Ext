using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users.Identifiers;

public class RoleIdentifier : Identifier
{
    private readonly Role _role;

    public RoleIdentifier(Role role)
    {
        _role = role;
    }

    public override IAsyncEnumerable<long> PrepareChatIdsAwait(IUsersProvider usersProvider)
        => usersProvider.GetAllAwait(_role);

    public override string ToString() => _role.ToString("G");
}