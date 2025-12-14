using System.ComponentModel;
using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users.Identifiers;

[TypeConverter(typeof(IdentifierTypeConverter))]
public abstract class Identifier
{
    private static readonly Dictionary<string, Func<string, Identifier?>> Parsers = new()
    {
        ["chat_id"] = ChatIdIdentifier.Parse,
        ["group"] = GroupIdentifier.Parse,
        ["group_list"] = GroupListIdentifier.Parse,
        // TODO add chat_topic_id
    };

    public static Identifier Parse(string input)
    {
        if (TryParse(input) is { } identifier)
            return identifier;
        throw new ArgumentOutOfRangeException(nameof(input), input, "Value should be number or Group");
    }

    public static Identifier? TryParse(string input)
    {
        if (long.TryParse(input, out var chatId))
            return FromChatId(chatId);

        var parts = input.Split(':', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
            return null;

        return Parsers.TryGetValue(parts[0], out var parser)
            ? parser(parts[1])
            : null;
    }

    public static Identifier FromChatId(long chatId) => new ChatIdIdentifier(new[] { chatId });
    public static Identifier FromChatIds(IEnumerable<long> chatIds) => new ChatIdIdentifier(chatIds);
    public static Identifier FromGroup(string group) => FromGroup(new Group(group));
    public static Identifier FromGroup(Group group) => new GroupIdentifier(group);

    public abstract IAsyncEnumerable<long> PrepareChatIdsAwait(IUsersProvider usersProvider);
}