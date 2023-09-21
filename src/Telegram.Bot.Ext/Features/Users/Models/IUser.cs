namespace Telegram.Bot.Ext.Features.Users.Models;

public enum Role
{
    User,
    Moderator,
    Administrator
}
public interface IUser
{
    long Id { get; }
    Role Role { get; }
}