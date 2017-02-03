"use strict";

var gulp = require("gulp-help")(require("gulp"));
var install = require("gulp-install");
var del = require("del");
var concat = require("gulp-concat");
var cssmin = require("gulp-cssmin");
var uglify = require("gulp-uglify");
var less = require("gulp-less");
var runseq = require("run-sequence");
var jf = require("jsonfile");
var modernizr = require("gulp-modernizr");
var through = require("through2");
var req = require("sync-request");
var Karma = require("karma").Server;
var tslint = require("gulp-tslint");
var eslint = require("gulp-eslint");
var argv = require("yargs").argv;
var path = require("path");
var shell = require("gulp-shell");
var bust = require("gulp-buster");
var assign = require("object-assign");
var replace = require("replace");
var fs = require("fs");
var exec = require("child_process").exec;
var os = require("os");
var es = require("event-stream");

const Builder = require("systemjs-builder");

var paths = {
    approot: "apps",
    webrootLabel: "wwwroot",
    staticLabel: "static",
    webroot: "./wwwroot/",
    testWebRoot: "./wwwroot-test/",
    staticRoot: "./static/",
    build: "./.build/",
    buildAot: "./.build/.aot/",
    aot: "./apps/.aot/"
};

var auiOptions = {
    sourcemaps: false,
    outputPath: "",
    watch: false
};

paths.js = paths.webroot + "js/**/*.js";
paths.staticJs = paths.staticRoot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.staticMinJs = paths.staticRoot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/dashboard.min.js";
paths.concatCssDest = paths.webroot + "css/dashboard.min.css";
paths.copyJsDest = paths.webroot + "js/";



// ==========
// Bound tasks
// ==========
gulp.task("set", "sets global variables", function (cb) { return runseq(["set:cache"], cb); });
gulp.task("min", "Minifies Js & Css", function (cb) { return runseq(["min:js", "min:css"], cb); });
gulp.task("bundle", "Bundles libs", function (cb) { return runseq(["bundle:rxjs"], cb); });
gulp.task("copy", "Copies built files to distribution", function (cb) { return runseq(["copy:javascript:libs", "copy:static", "copy:resources", "compile:modernizr:all"], cb); });
gulp.task("compile", "Compiles application", function (cb) { return runseq("compile:less:themes", "compile:ts:apps", cb); });
gulp.task("clean", "Cleans all output folders, including node_modules", function (cb) { return runseq(["clean:output", "clean:test"], cb); });
gulp.task("refresh:app", "Refreshes the app", function (cb) { return runseq(["min:js", "min:css", "compile:ts:apps", "compile:less:themes", "compile:less:apps", "copy:static", "copy:resources"], cb); });
gulp.task("test", "Builds app, runs tests", function (cb) { return runseq("clean:test", "test:prepare", "compile:ts:apps:spec", "copy:javascript:libs:test", "test:run", cb); });
gulp.task("build", "Builds app", function (cb) { return runseq("clean", "copy", "compile", "min", cb); });
gulp.task("build:jit", "Builds jit app", function (cb) { return runseq("clean", "copy:jit", "compile:jit", "min", "bundle", cb); });
gulp.task("default", "Builds application, copy, lint, compile, min", function (cb) { return runseq("lint", "build", cb); });

gulp.task("compile:jit", "Compiles jit application", function (cb) { return runseq(["compile:less:themes", "compile:less:apps", "compile:ts:apps:html", "compile:ts:jit"], cb); });

gulp.task("refresh:node", false, function (cb) {
    runseq("install:package", ["copy:javascript:lib", "copy:node:jasmine"], cb);
});

gulp.task("lint", "Lints the typescript and javascript files", function (cb) {
    runseq("lint:ts", "lint:js", cb);
});


// ==========
// translate
// ==========

gulp.task("translate", "Translates the resources into additional language files", function () {
    var eng = `./${paths.approot}/common/resources/locale-en_AU.json`;
    var engObj = jf.readFileSync(eng);
    var cleanResponse = function (data) {
        var quotePos = data.indexOf("\"");
        data = data.substr(quotePos + 1);
        quotePos = data.indexOf("\"");
        data = data.substring(0, quotePos);
        if (data === "zxx") data = "";
        return data;
    };

    return gulp.src([`${paths.approot}/common/resources/*.json`, `!${paths.approot}/common/resources/locale-en_AU.json`])
        .pipe(through.obj(function (file, enc, cb) {
            console.log(file.path);
            var lang = file.path.substr(file.path.length - 10, 5);
            var curObj = jf.readFileSync(file.path);
            for (var o in engObj) {
                if (engObj.hasOwnProperty(o)) {
                    if (curObj[o] === undefined) {
                        var sourceText = engObj[o];
                        console.log("translating to " + lang + " : " + sourceText);
                        var url = "http://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl=" + lang + "&dt=t&q=" + encodeURI(sourceText);
                        var res = req("GET", url);
                        var body = res.getBody("utf8");
                        var text = cleanResponse(body);
                        curObj[o] = text;
                    }
                }
            }

            jf.writeFileSync(file.path, curObj, { spaces: 2 });
            cb(null, file);
        }));
});

gulp.task("translate-prep", "Translates the resources into additional language files", function () {
    return gulp.src([`${paths.approot}/common/resources/*.json`, `!${paths.approot}/common/resources/locale-en_AU.json`])
        .pipe(through.obj(function (file, enc, cb) {
            console.log(file.path);
            jf.writeFileSync(file.path, {}, { spaces: 2 });
            cb(null, file);
        }));
});


// ==========
// clean
// ==========

gulp.task("clean:output", false, function (cb) {
    return del([paths.webroot + "**/*", paths.build + "**/*", paths.buildAot + "**/*", paths.aot + "**/*"]);
});

gulp.task("clean:test", false, function (cb) {
    return del([paths.testWebRoot + "**/*"]);
});



// ==========
// min
// ==========

gulp.task("min:js", false, function () {
    return gulp
        .src([
            paths.staticJs,
            "!" + paths.staticRoot + "js/**/*.min.js",
            "!" + paths.staticRoot + "js/shims_for_IE.js",
            "!" + paths.staticRoot + "js/angular2-polyfills.js",
            "!" + paths.staticRoot + "js/history.js/**/*",
            "!" + paths.staticRoot + "js/qr/qrcode.min.js"
        ], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", false, function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});



// ==========
// COPY
// ==========
// note: copy javascript required libs to wwwroot
gulp.task("copy:javascript:libs", false, function (cb) {
    return gulp.src(["node_modules/core-js/client/shim.min.js*",
        "node_modules/zone.js/dist/zone.min.js",
        "node_modules/reflect-metadata/Reflect.js*",
        "node_modules/traceur/**/*.js",
        "node_modules/moment/**/*.js",
        "node_modules/bootstrap/dist/js/bootstrap.min.js",
        "node_modules/bootstrap/dist/css/bootstrap.min.css*",
        "node_modules/bootstrap/dist/fonts/*",
        "node_modules/jquery/dist/jquery.min*",
        "node_modules/markdown/lib/markdown.js",
        "node_modules/intl-tel-input/build/**/*",
        "node_modules/systemjs/dist/system.js"
    ], { base: "node_modules" })
        .pipe(gulp.dest(paths.webroot + "js/"));
});

gulp.task("copy:javascript:libs:test", false, ["copy:node:jasmine"], function (cb) {
    return gulp.src(["node_modules/core-js/**/*",
        "node_modules/zone.js/**/*",
        "node_modules/reflect-metadata/**/*",
        "node_modules/traceur/**/*.js",
        "node_modules/moment/**/*.js",
        "node_modules/bootstrap/dist/**/*",
        "node_modules/jquery/dist/**/*",
        "node_modules/markdown/lib/markdown.js",
        "node_modules/intl-tel-input/build/**/*",
        "node_modules/systemjs/**/*",
        "node_modules/@angular/**/*",
        "node_modules/rxjs/**/*",
        "node_modules/zone.js/**/*"
    ], { base: "node_modules" })
        .pipe(gulp.dest(paths.testWebRoot + "js"));
});

gulp.task("copy:jit", false, ["copy"], function () {
    return gulp.src(["node_modules/@angular/**/*"], { base: "node_modules" })
        .pipe(gulp.dest("wwwroot/js"));
});

gulp.task("copy:node:jasmine", false, function () {
    return gulp.src(["node_modules/jasmine-core/**/*.js", "node_modules/jasmine-core/**/*.css"])
        .pipe(gulp.dest(paths.testWebRoot + "node_modules/jasmine-core"));
});

gulp.task("copy:static", false, function (cb) {
    return runseq(["copy:static:root", "copy:static:nonRoot"], cb);
});

gulp.task("copy:static:root", false, function () {
    return gulp.src([`${paths.staticLabel}/root/**/*`])
        .pipe(gulp.dest(paths.webroot));
});

gulp.task("copy:static:nonRoot", false, function () {
    return gulp.src([`${paths.staticLabel}/assets/**/*`, `${paths.staticLabel}/js/**/*`, `${paths.staticLabel}/css/**/*`], {
        base: `${paths.staticLabel}` })
        .pipe(gulp.dest(paths.webroot));
});

gulp.task("copy:tests:apps", false, function () {
    // copy the compiled main app and all the libs to the tests web app
    return gulp.src([paths.webroot + "**/*"])
        .pipe(gulp.dest(paths.testWebRoot));
});

gulp.task("copy:systemJs", false, function () {
    return es.concat(
        gulp.src(["systemjs.config.js"])
            .pipe(gulp.dest("wwwroot/js")),
        gulp.src(["node_modules/systemjs/**/*"], { base: "node_modules" })
            .pipe(gulp.dest("wwwroot/js")));
});

var correctHashLookup = function (hashObj) {
    var retVal = {};
    Object.keys(hashObj)
        .forEach(function (key) {
            var newKey = key.replace(paths.webroot, "");
            retVal[newKey] = hashObj[key];
        });
    return retVal;
};

gulp.task("copy:resources", false, function (cb) {
    var retVal = {};
    var transformer = function (hashObj) {
        retVal = correctHashLookup(hashObj);
        return retVal;
    };
    var opts = {
        transform: transformer
    };

    gulp.src([`${paths.approot}/common/resources/**/*`])
        .pipe(gulp.dest(paths.webroot + "resources/"))
        .on("end",
        () => {
            gulp.src([paths.webroot + "resources/*"])
                .pipe(bust(opts))
                .pipe(gulp.dest("./"))
                .on("end",
                () => {
                    cb();
                });
        });
});


// ==========
// Lint
// ==========
gulp.task("lint:ts:apps", false, function () {
    var format = argv.msbuild ? "msbuild" : "verbose";
    return gulp.src([`${paths.approot}/**/*.ts`])
        .pipe(tslint(format))
        .pipe(tslint.report({ emitError: true, summarizeFailureOutput: true }));
});

gulp.task("lint:js:dev", false, function () {
    return gulp.src(["*.js"])
        .pipe(eslint())
        .pipe(eslint.format())
        .pipe(eslint.failAfterError());
});

gulp.task("lint:js", false, function (cb) {
    runseq("lint:js:dev", cb);
});

gulp.task("lint:ts", false, function (cb) {
    runseq("lint:ts:apps", cb);
});


// ==========
// Compile
// ==========

gulp.task("compile:ts:aot", false, function (cb) {
    ngc(cb, `${paths.approot}/tsconfig.aot.json`);
});

gulp.task("compile:ts:jit", false, ["copy:systemJs", "compile:public:jit"], function (cb) {
    tsc(cb, `${paths.approot}/tsconfig.json`);
});

gulp.task("compile:ts:rollup:public", false, function (cb) {
    rollup(cb, `${paths.approot}/rollup-config-public.js`);
});

function rollup(cb, configPath) {
    var cmd = os.platform() === "win32" ? "node_modules\\.bin\\rollup" : "./node_modules/.bin/rollup";

    cmd = cmd + " -c " + configPath;

    exec(cmd, function (err, stdout, stderr) {
        console.log(stdout);
        console.log(stderr);
        cb(err);
    });
}

function ngc(cb, configPath) {
    var cmd = os.platform() === "win32" ? "node_modules\\.bin\\ngc" : "./node_modules/.bin/ngc";

    cmd = cmd + " -p " + configPath;

    exec(cmd, function (err, stdout, stderr) {
        console.log(stdout);
        console.log(stderr);
        cb(err);
    });
}

function tsc(cb, configPath) {
    var cmd = os.platform() === "win32" ? "node_modules\\.bin\\tsc" : "./node_modules/.bin/tsc";

    cmd = cmd + " -p " + configPath;

    exec(cmd, function (err, stdout, stderr) {
        console.log(stdout);
        console.log(stderr);
        cb(err);
    });
}

gulp.task("compile:ts:apps", false, ["compile:less:apps:in-place"], function (cb) {
    runseq("compile:ts:aot", "compile:ts:rollup:public", cb);
});

gulp.task("compile:ts:apps:html", false, function () {
    return gulp.src([`${paths.approot}/**/*.html`])
        .pipe(gulp.dest(`${paths.webrootLabel}/${paths.approot}`));
});

gulp.task("compile:ts:apps:spec", false, function (cb) {
    tsc(cb, `${paths.approot}/tsconfig.test.json`);
});

gulp.task("compile:less:themes", false, function (cb) {
    return gulp.src(["themes/**/*.less"])
        .pipe(less())
        .pipe(gulp.dest(`${paths.webrootLabel}/themes/`));
});

gulp.task("compile:less:apps:in-place", false, function (cb) {
    return gulp.src([`${paths.approot}/**/*.less`])
        .pipe(less())
        .pipe(gulp.dest(`${paths.approot}/`));
});

gulp.task("compile:less:apps", false, function (cb) {
    return gulp.src([`${paths.approot}/**/*.less`])
        .pipe(less())
        .pipe(gulp.dest(`${paths.webrootLabel}/${paths.approot}/`));
});

gulp.task("compile:modernizr:all", false, function (cb) {
    return gulp.src(["node_modules/modernizr/*/*.js"])
        .pipe(modernizr({
            tests: [
                "blobconstructor",
                "contenteditable",
                "vml",
                "webgl",
                "touch", [
                    "devicemotion",
                    "deviceorientation"
                ],
                "framed"
            ],
            options: ["setClasses"]
        }))
        .pipe(uglify())
        .pipe(gulp.dest(paths.webroot + "js/"));
});

gulp.task("compile:public:jit", false, function () {
    return gulp.src(["systemjs.config.js", `${paths.approot}/public.jit.js`])
        .pipe(concat("ng2app-public.min.js"))
        .pipe(gulp.dest(`${paths.webrootLabel}/`));
});

gulp.task("init:typings", shell.task("typings install"));



// ==========
// Bundle
// ==========

gulp.task("bundle:rxjs", false, function (cb) {
    // final NG2 has issues with RXJS no longer bundling, we need to do it ourselves
    // https://github.com/angular/angular/issues/9359
    var options = {
        normalize: true,
        runtime: false,
        sourceMaps: true,
        sourceMapContents: true,
        minify: true,
        mangle: false
    };
    var builder = new Builder("./");
    builder.config({
        paths: {
            "n:*": "node_modules/*",
            "rxjs/*": "node_modules/rxjs/*.js"
        },
        map: {
            "rxjs": "n:rxjs"
        },
        packages: {
            "rxjs": { main: "Rx.js", defaultExtension: "js" }
        }
    });

    return builder.bundle("rxjs", `${paths.webrootLabel}/js/rxjs/bundles/mdRx.js`, options);
});


// ==========
// install
// ==========

gulp.task("install:package", false, function (cb) {
    return gulp.src(["./package.json"])
        .pipe(install());
});



// ==========
// Tests
// ==========

gulp.task("test:prepare:traceur",
    function (cb) {
        return gulp.src(["node_modules/traceur/*/*.js"])
            .pipe(gulp.dest(paths.testWebRoot + "js/traceur"));
    });

gulp.task("test:prepare:systemjs",
    function (cb) {
        return gulp.src(["systemjs.config.js"])
            .pipe(gulp.dest(paths.testWebRoot));
    });

gulp.task("test:prepare", function (cb) { return runseq(["test:prepare:systemjs", "test:prepare:traceur"], cb); });

gulp.task("test:run", function (cb) {
    var options = {
        configFile: path.join(__dirname, "/karma.conf.js"),
        reporters: ["progress", "html", "coverage"]
    };

    if (argv.teamcity) {
        options.reporters = ["teamcity", "coverage"];
        options.coverageReporter = {
            type: "teamcity",
            dir: "coverage/"
        };
    }

    if (argv.jenkins) {
        options.reporters = ["coverage"];
        options.coverageReporter = {
            type: "cobertura",
            dir: "coverage/"
        };
    }

    if (argv.jenkins) {
        options.reporters.push("junit");
        options.junitReporter = {
            outputFile: path.join(__dirname, "../../artifacts/test-reports/client/Web.Client.xml")
        };
    }

    var server = new Karma(options, cb);
    console.log("starting tests");

    setTimeout(function () {
        server.start();
    }, 1000);
});

gulp.task("watch", "Watches the ts and less files for changes, then lints and compiles them", ["default"], function () {
    gulp.watch([`${path.approot}/**/*.ts`], ["lintThenCompile"]);
    gulp.watch(["*.js"], ["lint:js"]);
    gulp.watch([`${path.approot}/**/*.html`, `${path.approot}/**/*.less`], ["compile"]);
    gulp.watch(["themes/**/*.less"], ["compile:less:themes"]);
    gulp.watch([`${path.approot}/**/locale*.json`], ["copy:resources"]);
});

gulp.task("lintThenCompile", false,
    function (cb) {
        runseq("lint:ts", "compile", cb);
    });

gulp.task("pre-commit", ["lint"]);