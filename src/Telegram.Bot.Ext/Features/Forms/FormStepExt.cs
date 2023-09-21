using Telegram.Bot.Ext.Features.Forms.Steps;

namespace Telegram.Bot.Ext.Features.Forms;

public static class FormStepExt
{
    public static FormStep Cancellable(this FormStep step, string cancelCommand = "/cancel")
        => new CancelableFormStepDecorator(step, cancelCommand);
}