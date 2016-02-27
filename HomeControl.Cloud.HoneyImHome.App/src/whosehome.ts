import {autoinject} from 'aurelia-framework';
import {HttpClient } from 'aurelia-fetch-client';
import {EventAggregator } from 'aurelia-event-aggregator';
import {ShowHistoryCommand } from './ShowHistoryCommand';
import 'fetch';

@autoinject
export class Users {
    heading: string = 'Whos\e Home?';
    residents: any[] = [];
    ea: EventAggregator;
    selectedResident: string;
    token: string;

    constructor(private http: HttpClient, ea: EventAggregator) {
        console.log("creating users");
        this.configureHTTP();

        this.selectedResident = "";
        this.ea = ea;
        this.selectedResident = "";
    }

    private configureHTTP() {
        this.http.configure(config => {
            config
                .useStandardConfiguration()
                .withDefaults({
                    headers: {
                        'Authorization': 'Basic ' + this.token,
                    }
                })
            //.withBaseUrl('http://localhost:60756/api/State');
                .withBaseUrl('http://homecontrol-cloud-honeyimhome.azurewebsites.net/api/State');
        });

    }

    activate() {
        this.startRefresh();
        return this.refreshResidents();
    }

    startRefresh(): void {
        setInterval(() => {
            this.refreshResidents();
        }, 5000);
        //setInterval(this.refreshResidents, 5000);
    }

    refreshResidents(): any {
        //Console.log("ref");
        //this.residents.push({ name: "ER" });
        //if (this.token == null || this.token == "")
        //    return;

       this.http.fetch('')
            .then(response => response.json())
            .then(State => this.residents = State)
        .catch(error => this.residents = []);
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
}