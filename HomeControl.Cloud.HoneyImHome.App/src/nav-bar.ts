import {bindable} from 'aurelia-framework';
import {inject } from 'aurelia-framework';
import {AuthService } from 'paulvanbladel/aurelia-auth';

@inject(AuthService)

export class NavBar {
    // User isn't authenticated by default
    _isAuthenticated = false;
    @bindable router = null;

    auth: any;

    constructor(auth) {
        this.auth = auth;
  };

    // We can check if the user is authenticated
    // to conditionally hide or show nav bar items
    get isAuthenticated() {
        return this.auth.isAuthenticated();
  };
}