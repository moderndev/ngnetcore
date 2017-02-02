// ///<reference path="../../node_modules/@angular2/platform-browser-dynamic/index.d.ts"/>
///<reference path="../../../typings/index.d.ts"/>

import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { Title } from "@angular/platform-browser";
import { ErrorHandler, enableProdMode, Provider } from "@angular/core";
import { RouterModule } from "@angular/router";
import { LocationStrategy, HashLocationStrategy } from "@angular/common";
import { HttpModule, ConnectionBackend, XHRBackend, RequestOptions, Http } from "@angular/http";
import { PublicAppComponent } from "./app.component";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { routing } from "./app.routing";

console.log("begin public app bootstrap");

@NgModule({
    imports: [
        BrowserModule,
        HttpModule,
        FormsModule,
        ReactiveFormsModule,
        routing
    ],
    providers: [
        { provide: LocationStrategy, useClass: HashLocationStrategy }
    ],
    declarations: [
        PublicAppComponent
    ],
    bootstrap: [PublicAppComponent]
})

export class PublicAppModule {
    public constructor() {
    }
}
