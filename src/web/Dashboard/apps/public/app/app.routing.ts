import { Component, OnInit, OnDestroy, ModuleWithProviders } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { PublicLoginComponent } from "../../public/components/public-login/public-login.component";

const appRoutes: Routes = [
    {
       path: "",
       component: PublicLoginComponent
    },
    {
       path: "Home",
       component: PublicLoginComponent
    }
];

export const appRoutingProviders: any[] = [

];

export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes);