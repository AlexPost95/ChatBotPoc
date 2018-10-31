using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bonc_start.Dialogs.NewPostDialogs
{
    [Serializable]
    public class TwitterPostDialog : IDialog<object>
    {

        private string welcomeMessage = "Ik zal je een aantal stappen laten doorlopen om een tweet te kunnen plaatsen.";
        private string promptText = "Wat is de tekst van het bericht dat je zou willen posten op Twitter?";
        private string promptTextFail = "Kies een van de bovenstaande opties";
        private string tweetExample = "Je tweet ziet er als volgt uit:";

        private string twitterName = "Alex Post";
        private string twitterTag = "@AlexP";

        private string textToPost;


        /// <summary>
        /// Show welcome message and ask what text the user would like to add to the post.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(welcomeMessage);

            PromptDialog.Text(
                context: context,
                resume: ChoiceReceivedAsync,
                prompt: promptText,
                retry: promptTextFail
                );
        }

        /// <summary>
        /// Repeats the text and asks for an image url to add to the post.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual async Task ChoiceReceivedAsync(IDialogContext context, IAwaitable<string> text)
        {
            textToPost = await text;
            await context.PostAsync($"Ik heb de volgende tekst doorgekregen: '{textToPost}'");

            await context.PostAsync(tweetExample);

            var message = context.MakeMessage();
            var attachment = CreateTwitterCard(textToPost);
            message.Attachments.Add(attachment);
            await context.PostAsync(message);

            await context.PostAsync("Je wordt nu teruggestuurd naar het hoofdmenu");
            context.Call<object>(new Dialogs.WelcomeBackDialog(), DialogComplete);
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
        /// Create a new Twitter attachment.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public Attachment CreateTwitterCard(string text)
        {
            AdaptiveCard card = new AdaptiveCard()
            {
                Body = new List<CardElement>()
    {
        // Container
        new Container()
        {
            Items = new List<CardElement>()
            {
                // first column
                new ColumnSet()
                {
                    Columns = new List<Column>()
                    {
                        new Column()
                        {
                            Size = ColumnSize.Auto,
                            Items = new List<CardElement>()
                            {
                                new Image()
                                {
                                    Url = "http://icons.iconarchive.com/icons/gianni-polito/colobrush/256/software-crystal-msn-icon.png",
                                    Size = ImageSize.Small,
                                    Style = ImageStyle.Person
                                }
                            }
                        },
                        new Column()
                        {
                            Size = "300",

                            Items = new List<CardElement>()
                            {
                                new TextBlock()
                                {
                                    Text =  twitterName,
                                    Weight = TextWeight.Bolder,
                                    IsSubtle = true
                                },
                                 new TextBlock()
                                {
                                    Text =  twitterTag,
                                    Weight = TextWeight.Lighter,
                                    IsSubtle = true
                                }
                            }
                        }
                    }
                },
                new ColumnSet()
                {
                     Columns = new List<Column>()
                    {
                          new Column()
                        {
                            Size = ColumnSize.Auto,
                            Separation =SeparationStyle.Strong,
                            Items = new List<CardElement>()
                            {
                                new TextBlock()
                                {
                                    Text = text,
                                    Wrap = true
                                }
                            }
                        }
                    }
                }
            }
        }
     },
            };
            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
            return attachment;
        }
    }
}