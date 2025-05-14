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
        throw new ArgumentOutOfRangeException(nameof(input), input, "Value should be number or Group");
    }

    public static Identifier FromChatId(long chatId)
        => new ChatIdIdentifier(chatId);

    public static Identifier FromGroup(string group)
        => FromGroup(new Group(group));

    public static Identifier FromGroup(Group group)
        => new GroupIdentifier(group);

    public static Identifier? TryParse(string input)
    {
        if (long.TryParse(input, out var chatId))
            return FromChatId(chatId);

        var parts = input.Split(':', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
            return null;

        return parts[0] switch
        {
            "chat_id" => long.TryParse(parts[1].Trim(), out chatId) ? FromChatId(chatId) : null,
            "group" => FromGroup(parts[1]),
            _ => null
        };
    }

    public abstract IAsyncEnumerable<long> PrepareChatIdsAwait(IUsersProvider usersProvider);
}