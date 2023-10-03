namespace Telegram.Bot.Ext.Features.Users.Models;

public interface IUser
{
    long Id { get; }
    Role Role { get; }
}

public enum Role
{
    User,
    Moderator,
    Administrator
}
