using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Ext.Helpers;

public class InlineMarkup
{
    private readonly int _maxItemsInRow;
    private readonly List<List<InlineKeyboardButton>> _items = new();

    public InlineMarkup(int maxItemsInRow = 3)
    {
        _maxItemsInRow = maxItemsInRow;
    }

    public bool IsEmpty => _items.All(r => r.Count == 0);

    public InlineMarkup WithItem(string text, string callbackData)
        => WithItem(InlineKeyboardButton.WithCallbackData(text, callbackData));

    public InlineMarkup WithItem(InlineKeyboardButton button)
    {
        if (_items.Count == 0 || _items[^1].Count >= _maxItemsInRow)
            NewRow();

        _items[^1].Add(button);
        return this;
    }

    public InlineMarkup NewRow()
    {
        if (_items.Count == 0 || _items[^1].Count > 0)
            _items.Add(new List<InlineKeyboardButton>());
        return this;
    }

    public InlineKeyboardMarkup BuildMarkup() => new(_items);
}
