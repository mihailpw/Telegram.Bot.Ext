using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users.Providers;

public class UsersProviderToRepositoryAdapter<TUser> : IUsersProvider where TUser : IUser
{
    private readonly IUserRepository<TUser> _userRepository;
    private readonly IReadOnlyCollection<long> _admins;

    public UsersProviderToRepositoryAdapter(
        IUserRepository<TUser> userRepository,
        IReadOnlyCollection<long> admins)
    {
        _userRepository = userRepository;
        _admins = admins;
    }

    public async Task<bool> CheckIfAsync(long id, Role role)
    {
        if (role == Role.Administrator)
            return _admins.Contains(id);

        var user = await _userRepository.GetByIdAsync(id);
        return user?.Role == role;
    }

    public async Task<bool> CheckIfAsync(long id, Role[] oneOfRoles)
    {
        if (oneOfRoles.Contains(Role.Administrator))
            return _admins.Contains(id);

        var user = await _userRepository.GetByIdAsync(id);
        return user is not null && oneOfRoles.Contains(user.Role);
    }

    public async Task<Role?> GetRoleAsync(long id)
    {
        if (_admins.Contains(id))
            return Role.Administrator;

        var user = await _userRepository.GetByIdAsync(id);
        return user?.Role;
    }

    public IAsyncEnumerable<long> GetAllAwait(Role role)
    {
        return role == Role.Administrator
            ? _admins.ToAsyncEnumerable()
            : _userRepository.GetAllByRoleAwait(role).Select(u => u.Id);
    }

    public async Task<IReadOnlyCollection<long>> GetAllAsync(Role role)
    {
        if (role == Role.Administrator)
            return _admins;

        var users = await _userRepository.GetAllByRoleAsync(role);
        return users.Select(u => u.Id).ToList();
    }
}
