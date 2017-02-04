$(document).ready(function () {
    Promise.all(System.import("/js/rxjs/bundles/mdRx.js"), System.import("/apps/public/boot"))
        .then(null, console.error.bind(console));
});
