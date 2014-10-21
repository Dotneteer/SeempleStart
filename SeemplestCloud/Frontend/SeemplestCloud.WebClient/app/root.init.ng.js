var Main;
(function (Main) {
    // ------------------------------------------------------------------------
    // This variable holds the name of this application
    // ------------------------------------------------------------------------
    Main.appModuleName = 'SeemplestCloud';

    // ------------------------------------------------------------------------
    // This variable represents this application for the external world
    // ------------------------------------------------------------------------
    Main.appModule;

    // ------------------------------------------------------------------------
    // Initializes the Angular application. This method should be
    // called before initializing any other app component
    // ------------------------------------------------------------------------
    function initAngularApp() {
        Main.appModule = angular.module(Main.appModuleName, [
            'ngRoute',
            'ui.bootstrap',
            'Core',
            'Subscription'
        ]);
    }
    Main.initAngularApp = initAngularApp;
})(Main || (Main = {}));

Main.initAngularApp();
//# sourceMappingURL=root.init.ng.js.map
