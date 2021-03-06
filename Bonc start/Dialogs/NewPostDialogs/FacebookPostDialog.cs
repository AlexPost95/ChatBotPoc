﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Bonc_start.Dialogs.NewPostDialogs
{

    [Serializable]
    public class FacebookPostDialog : IDialog<object>
    {

        private string welcomeMessage = "Ik zal je een aantal stappen laten doorlopen om een bericht te plaatsen op Facebook.";
        private string promptText = "Wat is de tekst van het bericht dat je zou willen posten?";
        private string promptTextFail = "Kies een van de bovenstaande opties";

        private string textToPost;
        private string imageToPost;

        [NonSerialized]
        Timer t;

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

        public void timerEvent(object target)
        {
            t.Dispose();
            ConversationStarter.Resume(ConversationStarter.conversationId, ConversationStarter.channelId); //We don't need to wait for this, just want to start the interruption here
        }

        /// <summary>
        /// Repeats the text and asks for an image url to add to the post.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual async Task ChoiceReceivedAsync(IDialogContext context, IAwaitable<string> text)
        {
            var message = context.Activity;
            textToPost = await text;

            ConversationStarter.toId = message.From.Id;
            ConversationStarter.toName = message.From.Name;
            ConversationStarter.fromId = message.Recipient.Id;
            ConversationStarter.fromName = message.Recipient.Name;
            ConversationStarter.serviceUrl = message.ServiceUrl;
            ConversationStarter.channelId = message.ChannelId;
            ConversationStarter.conversationId = message.Conversation.Id;

            if (true)
            {
                context.Call<object>(new Dialogs.TestDialog(), DialogComplete);

            }
                
            await context.PostAsync($"Ik heb de volgende tekst doorgekregen: '{textToPost}'");

            t = new Timer(new TimerCallback(timerEvent));
            t.Change(5000, Timeout.Infinite);

            PromptDialog.Text(
                context: context,
                resume: PostCreatedAsync,
                prompt: "Plak de url van de toe te voegen afbeeling in het tekstvak.",
                retry: promptTextFail
                );
        }

        /// <summary>
        /// Creates a new Card with the image and text. 
        /// MakeAnalysisRequest is being called to provide the image with tags.
        /// After post is being displayed the user will be sent back to WelcomeBackDialog to start over
        /// </summary>
        /// <param name="context"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public virtual async Task PostCreatedAsync(IDialogContext context, IAwaitable<string> image)
        {
            t = new Timer(new TimerCallback(timerEvent));
            t.Change(5000, Timeout.Infinite);

            imageToPost = await image;

            await context.PostAsync("Ik heb de volgende Facebook-post voor je samengesteld:");
            var message = context.MakeMessage();
            var attachment = FacebookPostCard("", textToPost, imageToPost);
            message.Attachments.Add(attachment);
            await context.PostAsync(message);
                        
            await MakeAnalysisRequest(imageToPost, context);
            await context.PostAsync("Ik heb de afbeelding voor je getagd met keywords zodat de afbeelding de volgende keer makkelijk terug te vinden is.");
            await context.PostAsync("Je bericht is gepost. Wat zou je nu willen doen?");
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
        /// Creates an attachment for the Facebook post with title, text and imageUrl
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        private static Attachment FacebookPostCard(string title, string text, string imageUrl)
        {
            var heroCard = new HeroCard
            {
                Title = title,
                Subtitle = "",
                Text = text,
                Images = new List<CardImage> { new CardImage(imageUrl) },
            };
            return heroCard.ToAttachment();
        }

        /// <summary>
        /// Sends the image url to Microsoft Computer Vision Api and gets a json file in return with the image tags.
        /// </summary>
        /// <param name="imageFilePath"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        static async Task MakeAnalysisRequest(string imageFilePath, IDialogContext context)
        {
            const string subscriptionKey = "31495f44467e40e5aa8b017852219356";

            const string uriBase =
                "https://westeurope.api.cognitive.microsoft.com/vision/v2.0/analyze";

            try
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);
                
                string requestParameters =
                    "visualFeatures=Tags";
                //"visualFeatures=Categories,Description,Color";

                string uri = uriBase + "?" + requestParameters;

                HttpResponseMessage response;

                byte[] byteData = GetImageAsByteArray(imageFilePath);

                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    response = await client.PostAsync(uri, content);
                }

                string contentString = await response.Content.ReadAsStringAsync();

                Console.WriteLine("\nResponse:\n\n{0}\n",
                    JToken.Parse(contentString).ToString());
                
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
        }

        /// <summary>
        /// Converts the image to byte array.
        /// </summary>
        /// <param name="imageFilePath"></param>
        /// <returns></returns>
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (var webClient = new WebClient())
            {
                byte[] imageBytes = webClient.DownloadData(imageFilePath);
                return imageBytes;
            }
        }
    }
}