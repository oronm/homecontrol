import {autoinject} from 'aurelia-framework';
import {HttpClient } from 'aurelia-fetch-client';
import {EventAggregator } from 'aurelia-event-aggregator';
import {ShowHistoryCommand } from './ShowHistoryCommand';

import 'fetch';

@autoinject
export class residentHistory {
    public name: string = '';
    history: any[] = [];
    ea: EventAggregator;

    constructor(private http: HttpClient, name: string, ea: EventAggregator) {
        http.configure(config => {
            config
                .useStandardConfiguration()
                //.withBaseUrl('http://localhost:60756/api/State');
                .withBaseUrl('http://homecontrol-cloud-honeyimhome.azurewebsites.net/api/State');
        });
        //this.name = "Oron";
        this.name = name;
        this.ea = ea;
        this.ea.subscribe('showHistory', (command) => {
            console.log("event su " + command.name);
            this.name = command.name;
            this.refreshHistory();
        });
    }

    activate() {
        this.startRefresh();
        return this.refreshHistory();
    }

    startRefresh(): void {
        setInterval(() => {
            this.refreshHistory();
        }, 60000);
        //setInterval(this.refreshResidents, 5000);
    }

    refreshHistory(): any {
        //console.log("hist " + this.name);
        if (!this.name || this.name == "") {
            this.history = [];
            //console.log("returning");
            return;
        }
        //Console.log("ref");
        //this.residents.push({ name: "ER" });
        //console.log("fetching history for " + this.name);
        this.http.fetch("/" + this.name + '/History')
            .then(response => response.json())
            .then(History => this.history = History.history)
        .catch(error => this.history = []);
    }

    public actionDate(historyRecord: any): any {
        return historyRecord.wasPresent ? historyRecord.lastSeen : historyRecord.lastLeft;
    }
}