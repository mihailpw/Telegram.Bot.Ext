using System.ComponentModel;
using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users.Identifiers;

[TypeConverter(typeof(IdentifierTypeConverter))]
public abstract class Identifier
{
    public static Identifier Parse(string input)
    {
        if (TryParse(input, out var identifier))
            return identifier;
        throw new ArgumentOutOfRangeException(nameof(input), input, "Value should be number of Role");
    }

    public static bool TryParse(string input, out Identifier output)
    {
        if (Enum.TryParse<Role>(input, out var role))
        {
            output = new RoleIdentifier(role);
            return true;
        }

        if (long.TryParse(input, out var chatId))
        {
            output = new ChatIdIdentifier(chatId);
            return true;
        }

        output = default!;
        return false;
    }

    public abstract IAsyncEnumerable<long> PrepareChatIdsAwait(IUsersProvider usersProvider);
}