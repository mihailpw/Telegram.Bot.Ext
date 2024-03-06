using Telegram.Bot.Types;

namespace Example;

public class Commands
{
    public const string Hello = "hello";
    public const string HelloAdmin = "helloadmin";

    public static IEnumerable<BotCommand> ForUsers()
    {
        yield return new BotCommand { Command = Hello, Description = "Greet your bot" };
    }

    public static IEnumerable<BotCommand> ForAdmins()
    {
        foreach (var command in ForUsers())
            yield return new BotCommand { Command = command.Command, Description = $"[User] {command.Description}" };

        yield return new BotCommand { Command = HelloAdmin, Description = "[Admin] Greet from the admin throne" };
    }
}