import { platformBrowserDynamic } from "@angular/platform-browser-dynamic";
import { SharedFunctions } from "../common/utils/shared-functions";
import { PublicAppModule } from "./app/app.module";

SharedFunctions.enableProductionModeIfRequired();

platformBrowserDynamic().bootstrapModule(PublicAppModule)
    .then(success => console.log(`Bootstrap success`))
    .catch(error => console.log(error));