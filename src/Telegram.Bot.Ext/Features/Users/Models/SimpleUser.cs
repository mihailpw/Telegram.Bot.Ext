namespace Telegram.Bot.Ext.Features.Users.Models;

public class SimpleUser : IUser
{
    public SimpleUser(long id, Role role)
    {
        Id = id;
        Role = role;
    }

    public long Id { get; }
    public Role Role { get; }
}