using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users.Providers;

public sealed class NullUsersProvider : IUsersProvider
{
    public static readonly NullUsersProvider Instance = new();

    private NullUsersProvider() { }

    public Task<bool> CheckIfAsync(long id, Group group)
        => Task.FromResult(false);

    public Task<bool> CheckIfAsync(long id, Group[] oneOfGroups)
        => Task.FromResult(false);

    public Task<Group?> GetGroupAsync(long id)
        => Task.FromResult<Group?>(null);

    public Task<IReadOnlyCollection<Group>> GetGroupsAsync(long id)
        => Task.FromResult<IReadOnlyCollection<Group>>(Array.Empty<Group>());

    public IAsyncEnumerable<long> GetAllAwait()
        => AsyncEnumerable.Empty<long>();

    public IAsyncEnumerable<long> GetAllAwait(Group group)
        => AsyncEnumerable.Empty<long>();

    public Task<IReadOnlyCollection<long>> GetAllAsync(Group group)
        => Task.FromResult<IReadOnlyCollection<long>>(Array.Empty<long>());
}
