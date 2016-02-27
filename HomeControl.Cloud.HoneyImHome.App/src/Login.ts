import {autoinject} from 'aurelia-framework';
import {HttpClient } from 'aurelia-http-client';
import {EventAggregator } from 'aurelia-event-aggregator';
import 'fetch';

@autoinject
export class Login  {
    heading: string = 'Login';
    userName: string;
    password: string;
    token: string;
    errorMessage : string;
        
    constructor(private http: HttpClient, name: string, private ea: EventAggregator) {
        http.configure(config => {
            config
            .withBaseUrl('http://localhost:60756/api/State');
                //.withBaseUrl('http://homecontrol-cloud-honeyimhome.azurewebsites.net/api/State');
        });

    }

    submit(): void {
        var formData = new FormData();
        formData.append('username', this.userName);
        formData.append('password', this.password);

        this.errorMessage = "";
        this.http.createRequest('/Login')
            .asGet()
            .withParams({
                username: this.userName,
                password: this.password
            })
        .send()
        .then(response => this.handleLoginResult(JSON.parse(response.response)))
        .catch(error => this.errorMessage = JSON.parse(error.response).message);
    }

    handleLoginResult(responseToken: string) {
        this.token = responseToken;

        if (this.token == "" || this.token == null) this.errorMessage = this.errorMessage + " login failed";
    }


}