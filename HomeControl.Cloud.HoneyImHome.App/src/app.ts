import { RouterConfiguration, Router} from 'aurelia-router';
//import {HttpClientConfig} from 'paulvanbladel/aureliauth/app.httpClient.config';
//import {config } from './auth-config';

//@inject(HttpClientConfig)
export class App {
    router: Router;

    configureRouter(config: RouterConfiguration, router: Router) {  
    //configureRouter(config: RouterConfiguration, router: Router, httpClientConfig: HttpClientConfig) {  
        config.title = 'HIHome';
        //this.httpClientConfig = httpClientConfig;
        //this.httpClientConfig.configure();

        config.map([
            { route: ['login'], name: 'login', moduleId: './login', nav: true, title: 'Login' },
            { route: ['', 'whosehome'], name: 'whosehome', moduleId: './whosehome', nav: true, title: 'Whose Home' }
        ]);

        this.router = router;
    }
}