using Telegram.Bot.Ext.Utils;

namespace Telegram.Bot.Ext.Tests.Utils;

[TestOf(typeof(StringUtils))]
public class TelegramHtmlTests
{
    private TelegramHtml _sut;

    [SetUp]
    protected void OnSetUp()
    {
        _sut = new TelegramHtml();
    }

    [Test]
    public void WithText_TextWithHtmlSymbols_EscapesCorrectly()
    {
        _sut.WithText("1 < 2 & 3 > 1");
        var result = _sut.Build();

        Assert.That(result, Is.EqualTo("1 &lt; 2 &amp; 3 &gt; 1"));
    }

    [Test]
    public void WithBold_TextProvided_WrappedInBTag()
    {
        _sut.WithBold("bold text");
        var result = _sut.Build();

        Assert.That(result, Is.EqualTo("<b>bold text</b>"));
    }

    [Test]
    public void WithItalic_TextProvided_WrappedInITag()
    {
        _sut.WithItalic("italic text");
        var result = _sut.Build();

        Assert.That(result, Is.EqualTo("<i>italic text</i>"));
    }

    [Test]
    public void WithUnderline_TextProvided_WrappedInUTag()
    {
        _sut.WithUnderline("underlined");
        var result = _sut.Build();

        Assert.That(result, Is.EqualTo("<u>underlined</u>"));
    }

    [Test]
    public void WithStrikethrough_TextProvided_WrappedInSTag()
    {
        _sut.WithStrikethrough("struck");
        var result = _sut.Build();

        Assert.That(result, Is.EqualTo("<s>struck</s>"));
    }

    [Test]
    public void WithSpoiler_TextProvided_WrappedInSpoilerTag()
    {
        _sut.WithSpoiler("spoiler");
        var result = _sut.Build();

        Assert.That(result, Is.EqualTo("<tg-spoiler>spoiler</tg-spoiler>"));
    }

    [Test]
    public void WithInlineCode_TextProvided_WrappedInCodeTag()
    {
        _sut.WithInlineCode("code");
        var result = _sut.Build();

        Assert.That(result, Is.EqualTo("<code>code</code>"));
    }

    [Test]
    public void WithCodeBlock_LanguageProvided_WrappedInPreAndCodeTags()
    {
        _sut.WithCodeBlock("int x = 0;", "csharp");
        var result = _sut.Build();

        Assert.That(result, Is.EqualTo("<pre><code class=\"csharp\">int x = 0;</code></pre>"));
    }

    [Test]
    public void WithLink_ValidUrlAndTitle_CreatesAnchorTag()
    {
        _sut.WithLink("Google", "https://google.com");
        var result = _sut.Build();

        Assert.That(result, Is.EqualTo("<a href=\"https://google.com\">Google</a>"));
    }

    [Test]
    public void WithUserMention_ValidUserId_CreatesUserMentionTag()
    {
        _sut.WithUserMention("Mihail", 123456);
        var result = _sut.Build();

        Assert.That(result, Is.EqualTo("<a href=\"tg://user?id=123456\">Mihail</a>"));
    }

    [Test]
    public void WithNewLine_ForceAddIsTrue_AppendsNewLine()
    {
        _sut.WithText("Hello").WithNewLine(true).WithText("World");
        var result = _sut.Build();

        Assert.That(result, Is.EqualTo("Hello" + Environment.NewLine + "World"));
    }

    [Test]
    public void Build_TrailingNewLinesExist_TrimsByDefault()
    {
        _sut.WithText("Hello").WithNewLine().WithNewLine();
        var result = _sut.Build();

        Assert.That(result, Is.EqualTo("Hello"));
    }

    [Test]
    public void Build_TrailingNewLinesExistAndTrimmingDisabled_PreservesNewLines()
    {
        var html = new TelegramHtml(trimNewLines: false);

        html.WithText("Hello").WithNewLine().WithNewLine();
        var result = html.Build();

        Assert.That(result, Is.EqualTo("Hello" + Environment.NewLine));
    }

    [Test]
    public void If_ConditionIsTrue_ExecutesIfBranch()
    {
        _sut.If(true, b => b.WithText("yes"), b => b.WithText("no"));
        var result = _sut.Build();

        Assert.That(result, Is.EqualTo("yes"));
    }

    [Test]
    public void If_ConditionIsFalse_ExecutesElseBranch()
    {
        _sut.If(false, b => b.WithText("yes"), b => b.WithText("no"));
        var result = _sut.Build();

        Assert.That(result, Is.EqualTo("no"));
    }

    [Test]
    public void Row_MultipleRows_AppendsNewLinesAfterEach()
    {
        _sut.Row(b => b.WithText("line1"))
            .Row(b => b.WithText("line2"));
        var result = _sut.Build();

        Assert.That(result, Is.EqualTo("line1" + Environment.NewLine + "line2"));
    }

}