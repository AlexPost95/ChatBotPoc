using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bonc_start.Dialogs.NewPostDialogs
{
    [Serializable]
    public class NewPostDialog : IDialog<object>
    {

        private string welcomeMessage = "Je hebt gekozen om een nieuw bericht te plaatsen";
        private string promptText = "Ik zie dat je een nieuw bericht wil posten. Kies een van de volgende opties";
        private string promptTextFail = "Kies een van de bovenstaande opties";
 

    /// <summary>
    /// Shows welcome message and displays the options prompt.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(welcomeMessage);

            PromptDialog.Choice(
                context: context,
                resume: ChoiceReceivedAsync,
                options: (IEnumerable<Options>)Enum.GetValues(typeof(Options)),
                prompt: promptText,
                retry: promptTextFail,
                promptStyle: PromptStyle.Auto
                );
        }
          
        /// <summary>
        /// Waits for the user's choice and forwards to the correct dialog.
        /// For now only Facebook is implemented.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        public virtual async Task ChoiceReceivedAsync(IDialogContext context, IAwaitable<Options> activity)
        {
            Options response = await activity;
            if (response.ToString() == "Facebook")
            {
                context.Call<object>(new Dialogs.NewPostDialogs.FacebookPostDialog(), DialogComplete);
            }
            if (response.ToString() == "Twitter")
            {
                context.Call<object>(new Dialogs.NewPostDialogs.TwitterPostDialog(), DialogComplete);
            }
            if (response.ToString() == "Instagram")
            {
                await context.PostAsync("Deze functionaliteit is nog niet geimplementeerd.");
                context.Call<object>(new Dialogs.NewPostDialogs.NewPostDialog(), DialogComplete);
                //context.Call<object>(new Dialogs.NewPostDialogs.InstagramPostDialog(), DialogComplete);
            }
            if (response.ToString() == "LinkedIn")
            {
                await context.PostAsync("Deze functionaliteit is nog niet geimplementeerd.");
                context.Call<object>(new Dialogs.NewPostDialogs.NewPostDialog(), DialogComplete);
                //context.Call<object>(new Dialogs.NewPostDialogs.LinkedinPostDialog(), DialogComplete);
            }
            
        }

        /// <summary>
        /// Method to end the dialog. 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public virtual async Task DialogComplete(IDialogContext context, IAwaitable<object> response)
        {
            context.Done(this);
        }

        /// <summary>
        /// Platforms that the user can choose from in the prompt.
        /// Only Facebook implemented at this time.
        /// </summary>
        public enum Options
        {
            Facebook,
            Twitter,
            Instagram,
            LinkedIn
        }
    }
}