namespace Telegram.Bot.Ext.Features.Users.Models;

public class SimpleUser : IUser
{
    public SimpleUser(long id, Group group)
    {
        Id = id;
        Group = group;
    }

    public long Id { get; }
    public Group Group { get; }
}