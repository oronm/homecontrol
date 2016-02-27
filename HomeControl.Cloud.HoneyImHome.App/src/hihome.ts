import {Aurelia} from 'aurelia-framework';
import config from './auth-config';

export function configure(aurelia: Aurelia): void {
    console.log("configin");
    aurelia.use
        .standardConfiguration()
        //.plugin('paulvanbladel/aureliauth', (baseConfig) => {
        //    baseConfig.configure(config);
        //})
        .developmentLogging();

    aurelia.start().then(() => aurelia.setRoot());
}