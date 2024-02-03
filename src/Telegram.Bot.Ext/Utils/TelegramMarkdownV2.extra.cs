namespace Telegram.Bot.Ext.Utils;

// https://core.telegram.org/bots/api#markdownv2-style
public partial class TelegramMarkdownV2
{
    public static implicit operator string(TelegramMarkdownV2 builder) => builder.Build();

    public bool Equals(TelegramMarkdownV2? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return _trimNewLines == other._trimNewLines && _sb.Equals(other._sb);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((TelegramMarkdownV2)obj);
    }

    public override int GetHashCode() => HashCode.Combine(_trimNewLines, _sb);

    public override string ToString() => Build();

}
