import { Component, Input } from "@angular/core";
import {  } from "@angular/common";
import { RouterLink } from "@angular/router";


@Component({
    moduleId: module.id,
    selector: "public-login",
    templateUrl: "public-login.html",
    styleUrls: [ "public-login.css" ],
})
export class PublicLoginComponent {
    @Input() isRegisterUserHidden: boolean;

    constructor() {

    }

    ngOnInit() {

    }
}