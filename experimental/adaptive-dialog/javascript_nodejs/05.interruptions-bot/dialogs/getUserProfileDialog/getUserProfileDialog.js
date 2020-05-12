const { ComponentDialog } = require('botbuilder-dialogs');
const {LuisAdaptiveRecognizer, AdaptiveDialog, OnBeginDialog, SendActivity, TemplateEngineLanguageGenerator } = require('botbuilder-dialogs-adaptive');
const { Templates } = require('botbuilder-lg');
const { StringExpression } = require('adaptive-expressions');

const path = require('path');

const DIALOG_ID = 'GET_USER_PROFILE_DIALOG';

class GetUserProfileDialog extends ComponentDialog {
    constructor() {
        super(DIALOG_ID);
        const lgFile = Templates.parseFile(path.join(__dirname, 'getUserProfileDialog.lg'));
        const dialog = new AdaptiveDialog(DIALOG_ID).configure({
            generator: new TemplateEngineLanguageGenerator(lgFile),
            recognizer: this.createLuisRecognizer(),
            triggers: [
                new OnBeginDialog([
                    new SendActivity("In child dialog...")
                ])
            ]
        });

        // Add named dialogs to the DialogSet. These names are saved in the dialog state.
        this.addDialog(dialog);

        // The initial child Dialog to run.
        this.initialDialogId = DIALOG_ID;
    }

    createLuisRecognizer() {
        if (process.env.getUserProfileDialog_en_us_lu === "" || process.env.LuisAPIHostName === "" || process.env.LuisAPIKey === "")
            throw `Sorry, you need to configure your LUIS application and update .env file.`;
        return new LuisAdaptiveRecognizer().configure(
            {
                endpoint: new StringExpression(process.env.LuisAPIHostName),
                endpointKey: new StringExpression(process.env.LuisAPIKey),
                applicationId: new StringExpression(process.env.getUserProfileDialog_en_us_lu)
            }
        );
    }
}

module.exports.GetUserProfileDialog = GetUserProfileDialog;
