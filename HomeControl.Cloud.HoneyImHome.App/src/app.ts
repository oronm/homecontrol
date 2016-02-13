import { RouterConfiguration, Router} from 'aurelia-router';

export class App {
    router: Router;

    configureRouter(config: RouterConfiguration, router: Router) {
        config.title = 'HIHome';
        config.map([
            { route: ['', 'login'], name: 'login', moduleId: './login', nav: true, title: 'Login' },
            { route: 'whosehome', name: 'whosehome', moduleId: './whosehome', nav: true, title: 'Whos\'e Home' }
        ]);

        this.router = router;
    }
}