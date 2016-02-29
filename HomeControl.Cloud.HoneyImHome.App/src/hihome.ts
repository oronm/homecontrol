import {Aurelia} from 'aurelia-framework';
import authconfig from './auth-config';
import {autoinject } from 'aurelia-framework';

export function configure(aurelia: Aurelia): void {
    console.log("configin");
    aurelia.use
        .standardConfiguration()
        .developmentLogging()
        .plugin('paulvanbladel/aurelia-auth', (baseConfig) => {
            baseConfig.configure(authconfig);
        });

    aurelia.start().then(() => aurelia.setRoot());
}