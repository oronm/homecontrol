export class AppConfiguration {
    baseUri: string;
    historyAction: string;

    constructor() {
        this.baseUri = 'http://homecontrol-cloud-honeyimhome.azurewebsites.net/api/State';
        //this.baseUri = 'http://localhost:60756/api/State';

        this.historyAction = "History";
    }
}