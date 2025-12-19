using Moq;
using Telegram.Bot.Ext.Features.Messages;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Tests.Features.Remover;

[TestOf(typeof(InMemoryMessageRemover))]
public class InMemoryMessageRemoverTests : TestClassBase
{
    private static readonly ChatMessageId ChatMessageId1 = new(123, 321);

    [TestCase(-9999)]
    [TestCase(-999)]
    [TestCase(0)]
    [TestCase(999)]
    public async Task ScheduleForRemoval_SmallOrNegativeMilliseconds_RemovesInstantly(int ms)
    {
        DeleteMessageRequest? receivedDeleteRequest = null;
        Mocker.GetMock<ITelegramBotClient>()
            .Setup(b => b.MakeRequestAsync(It.IsAny<DeleteMessageRequest>(), It.IsAny<CancellationToken>()))
            .Callback((IRequest<bool> request, CancellationToken _) => receivedDeleteRequest = (DeleteMessageRequest)request)
            .Returns(Task.FromResult(true));
        var sut = Mocker.CreateInstance<InMemoryMessageRemover>();

        await sut.ScheduleForRemovalAsync([ChatMessageId1], TimeSpan.FromMilliseconds(ms));
        var scheduledForRemoval = await sut.CheckScheduledAsync(ChatMessageId1, token: CancellationToken.None);

        Assert.That(receivedDeleteRequest, Is.Not.Null);
        Assert.That(receivedDeleteRequest!.ChatId, Is.EqualTo(ChatMessageId1.ChatId));
        Assert.That(receivedDeleteRequest!.MessageId, Is.EqualTo(ChatMessageId1.MessageId));
        Assert.That(scheduledForRemoval, Is.EqualTo(false));
    }

    [Test]
    public async Task ScheduleForRemoval_1001Milliseconds_SchedulesForRemoval()
    {
        var sut = Mocker.CreateInstance<InMemoryMessageRemover>();

        await sut.ScheduleForRemovalAsync([ChatMessageId1], TimeSpan.FromMilliseconds(1001));
        var scheduledForRemoval = await sut.CheckScheduledAsync(ChatMessageId1, token: CancellationToken.None);

        Assert.That(scheduledForRemoval, Is.EqualTo(true));
    }
}
