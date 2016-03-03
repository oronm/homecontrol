import { autoinject, inject} from 'aurelia-framework';
import { RouterConfiguration, Router} from 'aurelia-router';
import {HttpClient } from 'aurelia-fetch-client';
import authconfig from './auth-config';
import 'fetch';
import { AuthorizeStep, FetchConfig, AuthService } from 'paulvanbladel/aurelia-auth';

// TODO: Refresh token?

@autoinject
export class App {
    router: Router;
    httpClientConfig: FetchConfig;
    httpClient: HttpClient;

    constructor(httpClientConfig: FetchConfig, httpClient: HttpClient) {
        this.httpClientConfig = httpClientConfig;
        this.httpClient = httpClient;
    }

    configureRouter(config: RouterConfiguration, router: Router) {  
        config.title = 'HIHome';

        this.httpClientConfig.configure();
        this.httpClient.configure(config => {
            config
                .withInterceptor({
                    response(response) {
                        if (response.status == 401 && response.url.indexOf(authconfig.loginUrl) <= 0) {
                            router.navigate("login");
                    }
                        return response;
                    }
                });

        });

        config.addPipelineStep('authorize', AuthorizeStep);

        config.map([
            { route: ['login'], name: 'login', moduleId: './login', nav: false, title: 'Login', authRoute: true },
            { route: ['', 'whosehome'], name: 'whosehome', moduleId: './whosehome', nav: true, title: 'Whose Home', auth: true },
            { route: 'logout', name: 'logout', moduleId: './logout', nav: false, title: 'Logout', authRoute: true }
        ]);

        this.router = router;
    }
}