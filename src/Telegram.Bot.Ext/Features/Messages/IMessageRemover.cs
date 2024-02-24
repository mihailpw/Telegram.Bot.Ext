using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Messages;

public interface IMessageRemover
{
    bool CheckScheduled(ChatId chatId, int messageId, string groupKey = "");
    bool CancelMessageRemoval(ChatId chatId, int messageId, string groupKey = "");
    void ScheduleForRemoval(ChatId chatId, int messageId, TimeSpan deleteIn, string groupKey = "");
    void RemoveAllImmediately(ChatId? chatId = null, int? messageId = null, string? groupKey = null);
}
