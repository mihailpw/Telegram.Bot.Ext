namespace Telegram.Bot.Ext.Features.Users.Identifiers;

public static class IdentifierExtensions
{
    public static async Task<IReadOnlyCollection<long>> GetChatIdsAsync(this Identifier identifier,
        IUsersProvider usersProvider)
        => await identifier
            .PrepareChatIdsAwait(usersProvider)
            .Distinct()
            .ToListAsync();

    public static async Task<IReadOnlyCollection<long>> GetChatIdsAsync(this IEnumerable<Identifier> identifiers,
        IUsersProvider usersProvider)
        => await identifiers.ToAsyncEnumerable()
            .SelectMany(id => id.PrepareChatIdsAwait(usersProvider))
            .Distinct()
            .ToListAsync();
}