using Moq.AutoMock;

namespace Telegram.Bot.Ext.Tests;

public abstract class TestClassBase
{
    protected AutoMocker Mocker { get; private set; } = null!;

    [SetUp]
    public void Setup()
    {
        Mocker = new AutoMocker();
        OnSetup();
    }

    protected virtual void OnSetup()
    {
    }
}
