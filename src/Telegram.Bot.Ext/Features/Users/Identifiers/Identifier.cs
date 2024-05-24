using System.ComponentModel;
using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users.Identifiers;

[TypeConverter(typeof(IdentifierTypeConverter))]
public abstract class Identifier
{
    public static Identifier Parse(string input)
    {
        if (TryParse(input) is { } identifier)
            return identifier;
        throw new ArgumentOutOfRangeException(nameof(input), input, "Value should be number or Role");
    }

    public static Identifier? TryParse(string input)
    {
        if (Enum.TryParse<Role>(input, out var role))
            return new RoleIdentifier(role);

        if (long.TryParse(input, out var chatId))
            return new ChatIdIdentifier(chatId);

        return null;
    }

    public abstract IAsyncEnumerable<long> PrepareChatIdsAwait(IUsersProvider usersProvider);
}