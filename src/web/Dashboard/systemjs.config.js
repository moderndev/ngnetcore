/**
 * System configuration for Angular 2 samples
 * Adjust as necessary for your application needs.
 */
(function(global) {
    // fixing
    var paths = {
        // paths serve as alias
        "npm:": "/js/"
    };

    // map tells the System loader where to look for things
    var map = {
        "apps/common": "/apps/common", // 'dist',
        "apps": "/apps/dashboard", // 'dist',
        "publicApp": "/apps/public", // 'dist'

        // other libraries
        // "rxjs": "npm:rxjs",
        "angular2-in-memory-web-api": "npm:angular2-in-memory-web-api",
        "traceur": "npm:traceur/src"
    };
    // packages tells the System loader how to load when no filename and/or no extension
    var packages = {
        "apps/common": { defaultExtension: "js" },
        "apps": { main: "boot.js", defaultExtension: "js" },
        "publicApp": { main: "boot.js", defaultExtension: "js" },
        "traceur": { main: "traceur.js", defaultExtension: "js" },
        "angular2-in-memory-web-api": { main: "index.js", defaultExtension: "js" }
    };
    var ngPackageNames = [
        "common",
        "compiler",
        "core",
        "forms",
        "http",
        "platform-browser",
        "platform-browser-dynamic",
        "router"
    ];
    // Individual files (~300 requests):
    function packIndex(pkgName) {
        // map["@angular/" + pkgName] = { main: "index.js", defaultExtension: "js" };
        map["@angular/" + pkgName] = "/js/@angular/" + pkgName + "/index.js?v=1";
    }
    // Bundled (~40 requests):
    function packUmd(pkgName) {
        // packages["@angular/" + pkgName] = { main: "bundles/" + pkgName + ".umd.min.js", defaultExtension: "js" };
        map["@angular/" + pkgName] = "/js/@angular/" + pkgName + "/bundles/" + pkgName + ".umd.min.js?v=1";
    }
    // Most environments should use UMD; some (Karma) need the individual index files
    var setPackageConfig = System.packageWithIndex ? packIndex : packUmd;
    // Add package entries for angular packages
    ngPackageNames.forEach(setPackageConfig);

    var config = {
        paths: paths,
        map: map,
        packages: packages,
        meta: {
            "js/rxjs/bundles/mdRx.js": {
                format: "global"
            }
        }
    };
    System.config(config);
})(this);
