using Moq;
using Telegram.Bot.Ext.Features.Messages;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Tests.Features.Remover;

[TestOf(typeof(MessageRemover))]
public class MessageRemoverTests : TestClassBase
{
    private static readonly (ChatId chatId, int messageId) ChatMessageId1 = (123, 321);
    private static readonly (ChatId chatId, int messageId) ChatMessageId2 = (234, 432);

    [TestCase(-9999)]
    [TestCase(-999)]
    [TestCase(0)]
    [TestCase(999)]
    public void ScheduleForRemoval_SmallOrNegativeMilliseconds_RemovesInstantly(int ms)
    {
        DeleteMessageRequest? receivedDeleteRequest = null;
        Mocker.GetMock<ITelegramBotClient>()
            .Setup(b => b.MakeRequestAsync(It.IsAny<DeleteMessageRequest>(), It.IsAny<CancellationToken>()))
            .Callback((IRequest<bool> request, CancellationToken _) => receivedDeleteRequest = (DeleteMessageRequest)request)
            .Returns(Task.FromResult(true));
        var sut = Mocker.CreateInstance<MessageRemover>();

        sut.ScheduleForRemoval(ChatMessageId1.chatId, ChatMessageId1.messageId, TimeSpan.FromMilliseconds(ms));
        var scheduledForRemoval = sut.CheckScheduled(ChatMessageId1.chatId, ChatMessageId1.messageId);

        Assert.That(receivedDeleteRequest, Is.Not.Null);
        Assert.That(receivedDeleteRequest!.ChatId, Is.EqualTo(ChatMessageId1.chatId));
        Assert.That(receivedDeleteRequest!.MessageId, Is.EqualTo(ChatMessageId1.messageId));
        Assert.That(scheduledForRemoval, Is.EqualTo(false));
    }

    [Test]
    public void ScheduleForRemoval_1001Milliseconds_SchedulesForRemoval()
    {
        var sut = Mocker.CreateInstance<MessageRemover>();

        sut.ScheduleForRemoval(ChatMessageId1.chatId, ChatMessageId1.messageId, TimeSpan.FromMilliseconds(1001));
        var scheduledForRemoval = sut.CheckScheduled(ChatMessageId1.chatId, ChatMessageId1.messageId);

        Assert.That(scheduledForRemoval, Is.EqualTo(true));
    }
}
