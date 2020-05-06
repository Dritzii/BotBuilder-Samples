using System.Collections.Generic;
using System.IO;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.LanguageGeneration;
using System.Security.Cryptography.Xml;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.BotBuilderSamples
{
    public class AddToDoDialog : ComponentDialog
    {
        private static IConfiguration Configuration;

        public AddToDoDialog(IConfiguration configuration)
            : base(nameof(AddToDoDialog))
        {
            Configuration = configuration;
            string[] paths = { ".", "Dialogs", "AddToDoDialog", "AddToDoDialog.lg" };
            string fullPath = Path.Combine(paths);
            // Create instance of adaptive dialog. 
            var AddToDoDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Generator = new TemplateEngineLanguageGenerator(Templates.ParseFile(fullPath)),
                // Create and use a LUIS recognizer on the child
                // Each child adaptive dialog can have its own recognizer. 
                Recognizer = CreateLuisRecognizer(),
                Triggers = new List<OnCondition>()
                {
                    new OnBeginDialog() 
                    {
                        Actions = new List<Dialog>()
                        {
                            // Take todo title if we already have it from root dialog's LUIS model.
                            // This is the title entity defined in ../RootDialog/RootDialog.lu.
                            // There is one LUIS application for this bot. So any entity captured by the rootDialog
                            // will be automatically available to child dialog.
                            // @EntityName is a short-hand for turn.entities.<EntityName>. Other useful short-hands are 
                            //     #IntentName is a short-hand for turn.intents.<IntentName>
                            //     $PropertyName is a short-hand for dialog.<PropertyName>
                            new SetProperties()
                            {
                                Assignments = new List<PropertyAssignment>()
                                {
                                    new PropertyAssignment()
                                    {
                                        Property = "dialog.itemTitle",
                                        Value = "=@itemTitle"
                                    },
                                    new PropertyAssignment()
                                    {
                                        Property = "dialog.listType",
                                        Value = "=@listType"
                                    }
                                }
                            },
                            // TextInput by default will skip the prompt if the property has value.
                            new TextInput()
                            {
                                Property = "dialog.todoTitle",
                                Prompt = new ActivityTemplate("${GetItemTitle()}"),
                                // This entity is coming from the local AddToDoDialog's own LUIS recognizer.
                                // This dialog's .lu file is under \AddToDoDialog\AddToDoDialog.lu
                                Value = "=@itemTitle",
                                // Allow interruption if we do not have an item title and have a super high confidence classification of an intent.
                                AllowInterruptions = "!@itemTitle && #Score >= 0.9"
                            },
                            // Get list type
                            new ChoiceInput()
                            {
                                Property = "dialog.listType",
                                Prompt = new ActivityTemplate("${GetListType()}"),
                                Choices = new ChoiceSet(new List<Choice>()
                                {
                                    new Choice("Todo"),
                                    new Choice("Shopping"),
                                    new Choice("Grocery")
                                }),
                                Value = "=@listType",
                                AllowInterruptions = "!@listType && #Score >= 0.8",
                                OutputFormat = "=toLower(this.value)"
                            },
                            // Add the new todo title to the list of todos. Keep the list of todos in the user scope.
                            new EditArray()
                            {
                                ItemsProperty = "user.lists[dialog.listType]",
                                ChangeType = EditArray.ArrayChangeType.Push,
                                Value = "=dialog.itemTitle"
                            },
                            new SendActivity("${AddItemReadBack()}")
                            // All child dialogs will automatically end if there are no additional steps to execute. 
                            // If you wish for a child dialog to not end automatically, you can set 
                            // AutoEndDialog property on the Adaptive Dialog to 'false'
                        }
                    },
                    // Handle local help
                    new OnIntent("Help")
                    {
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("${HelpAddItem()}")
                        }
                    }
                }
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(AddToDoDialog);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }

        private static Recognizer CreateLuisRecognizer()
        {
            if (string.IsNullOrEmpty(Configuration["LuisAppId"]) || string.IsNullOrEmpty(Configuration["LuisAPIKey"]) || string.IsNullOrEmpty(Configuration["LuisAPIHostName"]))
            {
                throw new Exception("Your LUIS application is not configured for AddToDoDialog. Please see README.MD to set up a LUIS application.");
            }
            return new LuisAdaptiveRecognizer()
            {
                Endpoint = Configuration["LuisAPIHostName"],
                EndpointKey = Configuration["LuisAPIKey"],
                ApplicationId = Configuration["LuisAppId-AddToDoDialog"]
            };
        }
    }
}
