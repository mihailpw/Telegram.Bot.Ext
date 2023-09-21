using Microsoft.Extensions.Logging;
using Telegram.Bot.Ext.Core;
using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users.Repositories;

public class FileUserRepository : IUserRepository<UserModel>
{
    private readonly SemaphoreSlim _semaphore = new(1);
    private readonly string _dbPath;
    private readonly ILogger<FileUserRepository> _logger;

    private class UserDataModel : UserModel
    {
        private UserDataModel(long id, Role role, string? userName, string name, DateTime created)
            : base(id, role, userName, name, created)
        {
        }

        public static UserModel? FromDataLine(string data)
        {
            var fields = data.Split(',');
            if (fields.Length != 6)
                return null;
            return new UserDataModel(
                long.Parse(fields[0]),
                (Role)int.Parse(fields[1]),
                fields[2],
                fields[3],
                DateTime.Parse(fields[4]))
            {
                IsActive = bool.Parse(fields[5])
            };
        }

        public static string ToDataLine(UserModel user)
        {
            return $"{user.Id},{user.Role:D},{user.UserName},{user.Name},{user.Created:O},{user.IsActive}";
        }
    }

    public FileUserRepository(string dbPath, ILogger<FileUserRepository> logger)
    {
        _dbPath = dbPath;
        Directory.CreateDirectory(Path.GetDirectoryName(_dbPath)!);
        _logger = logger;
        _logger.LogInformation("Users file path is {UsersFilePath}", _dbPath);
    }

    public async Task<bool> AddUserAsync(UserModel user)
    {
        using (await _semaphore.LockAsync())
        {
            var allUsers = await ReadAllAsync(true).ToListAsync();
            
            var existingUser = allUsers.Find(u => u.Id == user.Id);
            if (existingUser is { IsActive: true })
                return false;
            
            if (existingUser != null)
                existingUser.IsActive = true;
            else
                allUsers.Add(user);

            await SaveAllAsync(allUsers);
            return true;
        }
    }

    public IAsyncEnumerable<UserModel> GetAllAwait()
        => ReadAllAsync(false);

    public async Task<IReadOnlyCollection<UserModel>> GetAllByRoleAsync(Role role)
        => await ReadAllAsync(false).Where(u => u.Role == role).ToListAsync();

    public IAsyncEnumerable<UserModel> GetAllByRoleAwait(Role role)
        => ReadAllAsync(false).Where(u => u.Role == role);

    public async Task<UserModel?> GetByIdAsync(long id)
        => await ReadAllAsync(false).FirstOrDefaultAsync(u => u.Id == id);

    public async Task<UserModel?> GetByUserNameAsync(string userName)
        => await ReadAllAsync(false).FirstOrDefaultAsync(u => u.UserName == userName);

    public async Task<IReadOnlyCollection<UserModel>> GetAllAsync()
        => await ReadAllAsync(false).ToListAsync();

    public async Task<bool> DeleteUserAsync(long id)
    {
        using (await _semaphore.LockAsync())
        {
            var allUsers = await ReadAllAsync(true).ToListAsync();
            var index = allUsers.FindIndex(u => u.Id == id);
            if (index < 0 || !allUsers[index].IsActive)
                return false;
            allUsers[index].IsActive = false;
            await SaveAllAsync(allUsers);
            return true;
        }
    }

    private async IAsyncEnumerable<UserModel> ReadAllAsync(bool includeInactive)
    {
        if (!File.Exists(_dbPath))
            yield break;

        await using var stream = new FileStream(_dbPath, FileMode.Open, FileAccess.ReadWrite);
        using var reader = new StreamReader(stream);
        while (await reader.ReadLineAsync() is { } line)
        {
            var user = UserDataModel.FromDataLine(line);
            if (user == null)
                continue;

            if (user.IsActive || includeInactive)
                yield return user;
        }
    }

    private async Task SaveAllAsync(IEnumerable<UserModel> users)
    {
        var dataLines = users.Select(UserDataModel.ToDataLine);
        await File.WriteAllLinesAsync(_dbPath, dataLines);
        _logger.LogInformation("Users file updated");
    }
}
