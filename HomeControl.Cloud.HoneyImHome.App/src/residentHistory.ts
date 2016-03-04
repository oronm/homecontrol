import {autoinject} from 'aurelia-framework';
import {HttpClient } from 'aurelia-fetch-client';
import {EventAggregator } from 'aurelia-event-aggregator';
import {ShowHistoryCommand } from './ShowHistoryCommand';
import {AppConfiguration } from "./AppConfiguration";

import 'fetch';

@autoinject
export class residentHistory {
    public name: string = '';
    history: any[] = [];
    ea: EventAggregator;
    appconfig: AppConfiguration;
    refreshVar: any;

    constructor(private http: HttpClient, name: string, ea: EventAggregator, appconfig: AppConfiguration) {
        this.appconfig = appconfig;
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

    detached() {
        this.stopRefresh();
    }

    startRefresh(): void {
        this.stopRefresh();
        this.refreshVar = setInterval(() => {
            this.refreshHistory();
        }, this.appconfig.historyRefreshInterval);
    }

    stopRefresh(): void {
        var refresh = this.refreshVar;
        if (refresh) {
            clearInterval(refresh);
        }
    }


    refreshHistory(): any {
        if (!this.name || this.name == "") {
            this.history = [];
            return;
        }
        this.http.fetch(this.appconfig.baseUri + "/" + this.name + '/' + this.appconfig.historyAction)
            .then(response => response.json())
            .then(History => this.history = History.history)
        .catch(error => this.history = []);
    }

    public actionDate(historyRecord: any): any {
        return historyRecord.wasPresent ? historyRecord.lastSeen : historyRecord.lastLeft;
    }
}