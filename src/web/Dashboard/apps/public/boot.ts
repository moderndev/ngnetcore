import { platformBrowserDynamic } from "@angular/platform-browser-dynamic";
import { PublicAppModule } from "./app/app.module";

platformBrowserDynamic().bootstrapModule(PublicAppModule)
    .then(success => console.log(`Bootstrap success`))
    .catch(error => console.log(error));