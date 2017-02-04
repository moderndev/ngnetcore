import { NgModule, ANALYZE_FOR_ENTRY_COMPONENTS, ModuleWithProviders, ComponentFactoryResolver } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HttpModule, ConnectionBackend, XHRBackend, RequestOptions, Http } from "@angular/http";
import { RouterModule } from "@angular/router";
import { CapitalizePipe } from "../common/pipes/capitalize";

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        HttpModule,
        RouterModule
    ],
    declarations: [
        CapitalizePipe
    ],
    providers: [
        
    ],
    exports: [
        CapitalizePipe
    ],
    entryComponents: [

    ]
})
export class SharedModule {
}