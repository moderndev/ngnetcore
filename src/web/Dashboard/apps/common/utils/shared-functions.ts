import { Injectable, enableProdMode } from "@angular/core";

declare var jQuery: any;
declare let aconex: any;

export class SharedFunctions {


    static enableProductionModeIfRequired() {
        if (window && window.location && window.location.hostname) {
            SharedFunctions.enableProductionModeIfRequiredUsingHost(window.location.hostname);
        }
    }

    static enableProductionModeIfRequiredUsingHost(host: string) {
        if (host.endsWith(".com")) {
            enableProdMode();
            console.log("Running in production mode");
        }
        else {
            console.log("Running in development mode");
        }
    }
}