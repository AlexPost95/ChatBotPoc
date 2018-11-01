using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Bonc_start.Dialogs
{
    [Serializable]
    public class TestDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("test dialog");
            context.Wait(this.OptionsPrompt);
        }

        public virtual async Task OptionsPrompt(IDialogContext context, IAwaitable<IMessageActivity> activity)
        {
            await context.PostAsync("second method");
            //context.Done(this);
        }
    }
}