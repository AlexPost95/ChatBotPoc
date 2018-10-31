using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace Bonc_start.Dialogs
{
    [Serializable]
    public class WelcomeBackDialog : IDialog<object>
    {

        private string promptText = "Ik kan een aantal taken uitvoeren, wat zou je willen doen?";
        private string promptTextFail = "Die optie is niet beschikbaar. Kies een van de bovenstaande opties";


        /// <summary>
        /// Method that gets fires when this dialog is called. The prompt will give the user a number of choices.
        /// </summary>
        /// <param name="context"></param>
        public async Task StartAsync(IDialogContext context)
        {
            PromptDialog.Choice(
                context: context,
                resume:  ChoiceReceivedAsync,
                options: (IEnumerable<Options>)Enum.GetValues(typeof(Options)),
                prompt:  promptText,
                retry:   promptTextFail,
                promptStyle: PromptStyle.Auto
                );

            //return Task.CompletedTask;

        }

        /// <summary>
        /// Receives the option chosen in the previous method and calls the correct dialog.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="activity"></param>
        public virtual async Task ChoiceReceivedAsync(IDialogContext context, IAwaitable<Options> activity)
        {
            Options response = await activity;
            if (response.ToString() == "NieuwePost")
            {
                context.Call<object>(new Dialogs.NewPostDialogs.NewPostDialog(), DialogComplete);
            }
            if (response.ToString() == "Account")
            {
                await context.PostAsync("Deze functie is nog niet geimplementeerd.");
                context.Call<object>(new Dialogs.WelcomeBackDialog(), DialogComplete);
                //context.Call<object>(new Dialogs.AccountDialogs.AccountDialog(), DialogComplete);
            }
            if (response.ToString() == "BerichtenOverzicht")
            {
                await context.PostAsync("Deze functie is nog niet geimplementeerd.");
                context.Call<object>(new Dialogs.WelcomeBackDialog(), DialogComplete);
                //context.Call<object>(new Dialogs.PostOverviewDialogs.PostOverviewDialog(), DialogComplete);
            }
            if (response.ToString() == "Instructies")
            {
                await context.PostAsync("Deze functie is nog niet geimplementeerd.");
                context.Call<object>(new Dialogs.WelcomeBackDialog(), DialogComplete);
                //context.Call<object>(new Dialogs.InstructionDialogs.InstructionDialog(), DialogComplete);
            }
            if (response.ToString() == "Instellingen")
            {
                await context.PostAsync("Deze functie is nog niet geimplementeerd.");
                context.Call<object>(new Dialogs.WelcomeBackDialog(), DialogComplete);
                //context.Call<object>(new Dialogs.SettingsDialogs.SettingsDialog(), DialogComplete);
            }
        }

        /// <summary>
        /// Method to end this dialog.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="response"></param>
        public virtual async Task DialogComplete(IDialogContext context, IAwaitable<object> response)
        {
            context.Done(this);
        }

        /// <summary>
        /// Options for the user to choose from in 'StartAsync'.
        /// </summary>
        public enum Options
        {
            NieuwePost,
            Account,
            BerichtenOverzicht,
            Instructies,
            Instellingen
        }
    }
}