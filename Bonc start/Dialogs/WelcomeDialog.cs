using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Bonc_start.Dialogs
{
    [Serializable]
    public class WelcomeDialog : IDialog<object>
    {

        private string welcomeTitle = "Je persoonlijke social media assistent";
        private string welcomeMessage = "Hallo, ik ben Bonc, je persoonlijke social media assistent. Ik kan je leven een stuk " +
            "gemakkelijker maken op het gebied van social media, zo kan ik berichten voor je plaatsen, berichten schedulen en " +
            "je suggesties geven over wat je zou kunnen posten op je social media accounts.";
        private string promptText = "Heb je al een account of zou je er een willen maken?";
        private string promptTextFail = "Kies een van de bovenstaande opties";


        /// <summary>
        /// Show welcome card
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task StartAsync(IDialogContext context)
        {
            var message = context.MakeMessage();
            var attachment = WelcomeCard(welcomeTitle, welcomeMessage);
            message.Attachments.Add(attachment);
            await context.PostAsync(message);

            context.Wait(this.OptionsPrompt);
        }

        /// <summary>
        /// Show the prompt where the user can choose to log in or create a new account
        /// </summary>
        /// <param name="context"></param>
        /// <param name="activity"></param>
        public virtual async Task OptionsPrompt(IDialogContext context, IAwaitable<IMessageActivity> activity)
        {

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
        /// Method that waits for the input of 'OptionsPrompt' and guides the user to the correct dialog
        /// </summary>
        /// <param name="context"></param>
        /// <param name="activity"></param>
        public virtual async Task ChoiceReceivedAsync(IDialogContext context, IAwaitable<Options> activity)
        {
            Options response = await activity;

            if (response.ToString() == "LogIn")
            {
                context.Call<object>(new Dialogs.WelcomeBackDialog(), DialogComplete);
            }
            if (response.ToString() == "NewAccount")
            {
                context.Call<object>(new Dialogs.NewAccountDialog(), DialogComplete);
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
        /// Options to show in the prompt
        /// </summary>
        public enum Options
        {
            LogIn,
            NewAccount
        }

        /// <summary>
        /// Welcome card attachment, gets called in StartAsync.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private static Attachment WelcomeCard(string title, string text)
        {
            var heroCard = new HeroCard
            {
                Title = title,
                Subtitle = "",
                Text = text,
                Images = new List<CardImage> { new CardImage("https://d1dh93s7n44ml6.cloudfront.net/blog/wp-content/uploads/2017/04/14095449/my-virtual-assistant.jpg") },
            };
            return heroCard.ToAttachment();
        }
    }
}