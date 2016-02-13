import {autoinject} from 'aurelia-framework';
import {HttpClient } from 'aurelia-fetch-client';
import 'fetch';

@autoinject
export class Users {
    heading: string = 'Whos\e Home?';
    residents: any[] = [];

    constructor(private http: HttpClient) {
        http.configure(config => {
            config
                .useStandardConfiguration()
                .withBaseUrl('http://localhost:60756/api/');
        });
    }

    activate() {
        this.startRefresh();
        return this.refreshResidents();
    }

    startRefresh(): void {
        setInterval(() => {
            this.refreshResidents();
        }, 60000);
        //setInterval(this.refreshResidents, 5000);
    }

    refreshResidents(): any {
        //Console.log("ref");
        //this.residents.push({ name: "ER" });
        this.http.fetch('State')
            .then(response => response.json())
            .then(State => this.residents = State)
        .catch(error => this.residents = []);
    }
}