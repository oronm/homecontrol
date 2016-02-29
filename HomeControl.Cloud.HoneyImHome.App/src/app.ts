import { autoinject, inject} from 'aurelia-framework';
import { RouterConfiguration, Router} from 'aurelia-router';
import 'fetch';
import { AuthorizeStep, FetchConfig, AuthService } from 'paulvanbladel/aurelia-auth';
//import {FetchConfig } from 'paulvanbladel/aurelia-auth/app.fetch-httpClient.config';

//import {HttpClientConfig} from 'paulvanbladel/aureliauth/app.httpClient.config';
//import {config } from './auth-config';

@autoinject
export class App {
    router: Router;
    httpClientConfig: FetchConfig;
    constructor(httpClientConfig: FetchConfig) {
        this.httpClientConfig = httpClientConfig;
    }

    configureRouter(config: RouterConfiguration, router: Router) {  
    //configureRouter(config: RouterConfiguration, router: Router, httpClientConfig: HttpClientConfig) {  
        config.title = 'HIHome';
        //console.log(new FetchConfig());
        //console.log(FetchConfig);

        this.httpClientConfig.configure();

        config.addPipelineStep('authorize', AuthorizeStep);

        config.map([
            { route: ['login'], name: 'login', moduleId: './login', nav: false, title: 'Login', authRoute: true },
            { route: ['', 'whosehome'], name: 'whosehome', moduleId: './whosehome', nav: true, title: 'Whose Home', auth: true },
            { route: 'logout', name: 'logout', moduleId: './logout', nav: false, title: 'Logout', authRoute: true }
        ]);

        this.router = router;
    }
}