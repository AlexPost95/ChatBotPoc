using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Bonc_start.Dialogs
{
    [Serializable]
    public class NewAccountDialog : IDialog<object>
    {

        private string name;
        private string companyName;
        private string email;


        /// <summary>
        /// Method that gets called when the dialog is being entered. 
        /// Shows an information message and asks the user for its name.
        /// </summary>
        /// <param name="context"></param>
        public Task StartAsync(IDialogContext context)
        {
            context.PostAsync("Hallo, mijn naam is Bonc. Ik kan je met van alles helpen op het gebied van social media, zo kan je via mij berichten plaatsen op " +
                              "Facebook, Instagram, Twitter en LinkedIn. Om een nieuw account aan te maken heb ik een aantal gegevens nodig");
            PromptDialog.Text(
            context: context,
            resume: ResumeGetName,
            prompt: "Laat me je een paar vragen stellen om je account op te bouwen. Wat is je naam?"
            );
            return Task.CompletedTask;
        }

        /// <summary>
        /// Waits until the user inputs its name and asks for which company the account is for.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Username"></param>
        public virtual async Task ResumeGetName(IDialogContext context, IAwaitable<string> Username)
        {
            string response = await Username;
            name = response;
            
            PromptDialog.Text(
                context: context,
                resume: ResumeGetCompanyName,
                prompt: $"Aangenaam, {name}. Voor welk bedrijf zou je Bonc willen gebruiken?",
                retry: "Sorry, I didn't understand that. Please try again."
            );
        }

        /// <summary>
        /// Displays the name and company name. Asks for email address to register the account with.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Companyname"></param>
        public virtual async Task ResumeGetCompanyName(IDialogContext context, IAwaitable<string> Companyname)
        {
            string response = await Companyname;
            companyName = response;
            await context.PostAsync($"Cool, we zijn blij je als klant te mogen verwelkomen {companyName}");

            PromptDialog.Text(
                context: context,
                resume: ResumeGetEmail,
                prompt: $"Wat is het emailadres waarmee je een Bonc-account zou willen maken voor {companyName}, {name}?",
                retry: "Geef een geldig emailadres op."
            );
        }

        /// <summary>
        /// Checks if email address if valid. If valid then the user is being forwarded to WelcomeBackDialog, if not the user is asked to enter again.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Email"></param>
        public virtual async Task ResumeGetEmail(IDialogContext context, IAwaitable<string> Email)
        {
            string response = await Email;
            email = response;
            if (IsValidEmail(email))
            {
                await context.PostAsync($"Bedankt voor de informatie, je nieuwe account is aangemaakt.");

                var replyMessage = context.MakeMessage();
                var att = CreateAdapativecardWithColumn();
                replyMessage.Attachments.Add(att);
                await context.PostAsync(replyMessage);

                await context.PostAsync($"Je wordt nu ingelogd en dan kan je beginnen om Bonc te gebruiken.");
                context.Call<object>(new Dialogs.WelcomeBackDialog(), DialogComplete);
            }
            else
            {
                PromptDialog.Text(
                context: context,
                resume: ResumeGetCompanyName,
                prompt: $"Geef een geldig emailadres op.",
                retry: "Geef een geldig emailadres op."
            );
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

        public Attachment CreateAdapativecardWithColumn()
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
                                    Size = ImageSize.Medium,
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
                                    Text =  "Naam: " + name,
                                    Weight = TextWeight.Bolder,
                                    IsSubtle = true
                                },
                                 new TextBlock()
                                {
                                    Text =  "Bedrijfsnaam: " + companyName,
                                    Weight = TextWeight.Lighter,
                                    IsSubtle = true
                                },
                                  new TextBlock()
                                {
                                    Text =  "Emailadres: " + email,
                                    Weight = TextWeight.Lighter,
                                    IsSubtle = true
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

        /// <summary>
        /// Check if user input is a valid email address
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns>True or false</returns>
        bool IsValidEmail(string emailAddress)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(emailAddress);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}