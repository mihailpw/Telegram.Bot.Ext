namespace Telegram.Bot.Ext.Features.Users.Models;

public class UserModel : IUser
{
    protected UserModel(long id, Role role, string? userName, string name, DateTime created)
    {
        Id = id;
        Role = role;
        UserName = userName;
        Name = name;
        Created = created;
    }

    public long Id { get; }
    public Role Role { get; }
    public string? UserName { get; }
    public string Name { get; }
    public DateTime Created { get; }
    public bool IsActive { get; set; }

    public static UserModel Create(long id, Role role, string? userName, string name)
    {
        return new UserModel(id, role, userName, name, DateTime.UtcNow);
    }
}