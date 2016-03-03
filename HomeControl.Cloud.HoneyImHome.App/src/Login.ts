import {AuthService } from 'paulvanbladel/aurelia-auth';
import {inject } from 'aurelia-framework';

@inject(AuthService)

export class Login {

    heading = 'Login';

    // User inputs will be bound to these view models
    // and when submitting the form for login  
    email = '';
    password = '';
    working: boolean;

    // This view model will be given an error value
    // if anything goes wrong with the login
    loginError = '';

    auth: AuthService;

    constructor(auth) {
        this.auth = auth;
        this.working = false;
  };

    login() {
        this.working = true;

        return this.auth.login(this.email, this.password)
            .then(response => {
            })
            .catch(error => {
                this.working = false;
                this.loginError = "Login Failed";
            });
  };
}