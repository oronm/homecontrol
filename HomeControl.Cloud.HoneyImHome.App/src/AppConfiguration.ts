export class AppConfiguration {
    baseUri: string;
    historyAction: string;
    stateRefreshInterval: number;
    historyRefreshInterval: number;
    staleInMinutes = 60;

    constructor() {
        this.baseUri = 'http://homecontrol-cloud-honeyimhome.azurewebsites.net/api/State';
        //this.baseUri = 'http://localhost:60756/api/State';

        this.historyAction = "History";

        this.stateRefreshInterval = 60000;
        this.historyRefreshInterval = this.stateRefreshInterval;
    }
}