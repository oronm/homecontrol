 //Specific settings for our application's
 //authentication context. These will override
 //the default settings provided by aureliauth
import {AppConfiguration } from "./AppConfiguration";

var authconfig = {

    // Our Node API is being served from localhost:3001
    //baseUrl : 'http://homecontrol-cloud-honeyimhome.azurewebsites.net/api/State',
    baseUrl: new AppConfiguration().baseUri,
    // The API specifies that new users register at the POST /users enpoint
    signupUrl: 'users',
    // Logins happen at the POST /sessions/create endpoint
    loginUrl: 'CreateTokenEmail',
    // The API serves its tokens with a key of id_token which differs from
    // aureliauth's standard
    tokenName: 'access_token',
    // Once logged in, we want to redirect the user to the welcome view
    loginRedirect: '#/whosehome',

}

export default authconfig;
