using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Utils;

public static class UpdateExt
{
    public static long? GetUserId(this Update update)
        => update switch
        {
            { CallbackQuery.From.Id: var userId } => userId,
            { ChannelPost.From.Id: var userId } => userId,
            { ChatJoinRequest.From.Id: var userId } => userId,
            { ChatMember.From.Id: var userId } => userId,
            { ChosenInlineResult.From.Id: var userId } => userId,
            { EditedChannelPost.From.Id: var userId } => userId,
            { EditedMessage.From.Id: var userId } => userId,
            { InlineQuery.From.Id: var userId } => userId,
            { Message.From.Id: var userId } => userId,
            { MyChatMember.From.Id: var userId } => userId,
            // { Poll: not null } => null,
            { PollAnswer.User.Id: var userId } => userId,
            { PreCheckoutQuery.From.Id: var userId } => userId,
            { ShippingQuery.From.Id: var userId } => userId,
            _ => null
        };

    public static ChatId? GetChatId(this Update update)
        => update switch
        {
            { CallbackQuery.Message.Chat.Id: var chatId } => chatId,
            { ChannelPost.Chat.Id: var chatId } => chatId,
            { ChatJoinRequest.Chat.Id: var chatId } => chatId,
            { ChatMember.Chat.Id: var chatId } => chatId,
            // { ChosenInlineResult: not null } => null,
            { EditedChannelPost.Chat.Id: var chatId } => chatId,
            { EditedMessage.Chat.Id: var chatId } => chatId,
            { InlineQuery.From.Id: var chatId } => chatId,
            { Message.Chat.Id: var chatId } => chatId,
            { MyChatMember.Chat.Id: var chatId } => chatId,
            // { Poll: not null } => null,
            // { PollAnswer: not null } => null,
            // { PreCheckoutQuery: not null } => null,
            // { ShippingQuery: not null } => null,
            _ => null
        };

    public static int? GetMessageId(this Update update)
        => update switch
        {
            { CallbackQuery.Message.MessageId: var messageId } => messageId,
            { ChannelPost.MessageId: var messageId } => messageId,
            // { ChatJoinRequest: not null } => null,
            // { ChatMember: not null } => null,
            // { ChosenInlineResult: not null } => null,
            { EditedChannelPost.MessageId: var messageId } => messageId,
            { EditedMessage.MessageId: var messageId } => messageId,
            // { InlineQuery: not null } => null,
            { Message.MessageId: var messageId } => messageId,
            // { MyChatMember: not null } => null,
            // { Poll: not null } => null,
            // { PollAnswer: not null } => null,
            // { PreCheckoutQuery: not null } => null,
            // { ShippingQuery: not null } => null,
            _ => null
        };
}
