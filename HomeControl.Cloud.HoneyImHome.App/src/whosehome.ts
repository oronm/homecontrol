import {autoinject} from 'aurelia-framework';
import {HttpClient } from 'aurelia-fetch-client';
import {EventAggregator } from 'aurelia-event-aggregator';
import {ShowHistoryCommand } from './ShowHistoryCommand';
import {AppConfiguration } from "./AppConfiguration";
//import { FetchConfig } from 'paulvanbladel/aurelia-auth';
import 'fetch';

@autoinject
export class Users {
    heading: string = 'Whos\e Home?';
    residents: any[] = [];
    ea: EventAggregator;
    selectedResident: string;
    token: string;
    appconfig: AppConfiguration;
    refreshVar: any;
    isStale: boolean;

    constructor(private http: HttpClient, ea: EventAggregator, appconfig: AppConfiguration) {
        console.log("creating users");
        this.appconfig = appconfig;

        this.selectedResident = "";
        this.ea = ea;
        this.selectedResident = "";
        this.isStale = false;
    }

    activate() {
        this.startRefresh();
        return this.refreshResidents();
    }

    detached() {
        this.stopRefresh();
    }


    startRefresh(): void {
        this.stopRefresh();
        this.refreshVar = setInterval(() => {
            this.refreshResidents();
        }, this.appconfig.stateRefreshInterval);
    }

    stopRefresh(): void {
        var refresh = this.refreshVar;
        if (refresh) {
            clearInterval(refresh);
        }
    }

    refreshResidents(): any {
        this.http.fetch(this.appconfig.baseUri)
            .then(response => response.json())
            .then(State => { this.residents = State; this.updateIsStale(); })
            .catch(error => { this.residents = []; this.updateIsStale(); });
    }

    showHistory(name: string) {
        if (!name || name == "")
            this.hideHistory();
        else {
            this.selectedResident = name;
            this.ea.publish("showHistory", new ShowHistoryCommand(name));
            console.log("event pushed");
        }
    }

    hideHistory() {
        this.selectedResident = "";
        this.ea.publish("showHistory", new ShowHistoryCommand(""));
    }
    toggleHistory(name: string) {
        console.log("toggling");
        if (this.selectedResident == name)
            this.hideHistory();
        else
            this.showHistory(name);
    }

    hasBeenSeen(resident): boolean {
        return (resident && resident.lastSeen && new Date(resident.lastSeen).getYear() > 0);
    }

    updateIsStale() {
        this.isStale =
            !this.residents ||
            this.residents.every(resident => this.isResidentStale(resident));
    }

    isResidentStale(resident): boolean {
        if (!resident || !(resident.lastSeen && resident.lastLeft))
            return true;

        var elapsedSeen = (Date.now() - Date.parse(resident.lastSeen)) / 1000;
        var elapsedLeft = (Date.now() - Date.parse(resident.lastLeft)) / 1000;

        var staleInSeconds = this.appconfig.staleInMinutes * 60;

        return elapsedLeft >= staleInSeconds || elapsedSeen > staleInSeconds;
        
    }
}