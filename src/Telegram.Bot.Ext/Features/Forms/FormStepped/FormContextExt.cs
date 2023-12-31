namespace Telegram.Bot.Ext.Features.Forms.FormStepped;

public static class SteppedFormContextExt
{
    public static TBag GetBag<TBag>(this IFormContext target) where TBag : IFormBag
        => (TBag) target.Bag;
}