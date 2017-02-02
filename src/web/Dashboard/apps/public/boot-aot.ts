import { PublicAppModuleNgFactory } from "../aot/public/app/app.module.ngfactory";
import { platformBrowser } from "@angular/platform-browser";

// Compile and launch the module
const platform = platformBrowser();
platform.bootstrapModuleFactory(PublicAppModuleNgFactory);
