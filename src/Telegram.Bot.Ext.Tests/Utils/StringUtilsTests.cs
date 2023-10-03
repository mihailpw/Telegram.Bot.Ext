using Telegram.Bot.Ext.Utils;

namespace Telegram.Bot.Ext.Tests.Utils;

[TestOf(typeof(StringUtils))]
public class StringUtilsTests : TestClassBase
{
    [TestCase(null, ExpectedResult = false)]
    [TestCase("test", ExpectedResult = false)]
    [TestCase("/test", ExpectedResult = true)]
    [TestCase("/test 123", ExpectedResult = true)]
    [TestCase("/test123", ExpectedResult = true)]
    public bool IsCommand_(string? text)
    {
        return StringUtils.IsCommand(text);
    }

    [TestCase("/test", "test", ExpectedResult = true)]
    [TestCase("/test 123", "test", ExpectedResult = true)]
    [TestCase("/test", "test2", ExpectedResult = false)]
    [TestCase("/test2", "test", ExpectedResult = false)]
    public bool IsExactCommand_Valid_ReturnCorrect(string? text, string command)
    {
        return StringUtils.IsExactCommand(text, command);
    }

    [Test]
    public void IsExactCommand_EmptyCommand_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => StringUtils.IsExactCommand(null, ""));
    }

    [TestCase("test", ExpectedResult = "/test")]
    [TestCase("command", ExpectedResult = "/command")]
    [TestCase("/test", ExpectedResult = "/test")]
    [TestCase("test 1234", ExpectedResult = "/test")]
    [TestCase("/test 1234", ExpectedResult = "/test")]
    public string PrepareCommand(string command)
    {
        return StringUtils.PrepareCommand(command);
    }

    [Test]
    public void PrepareCommand_EmptyCommand_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => StringUtils.PrepareCommand(""));
    }
}
