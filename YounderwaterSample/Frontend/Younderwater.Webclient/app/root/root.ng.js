var Main;
(function (Main) {
    Main.appModuleName = 'Younderwater';
    /*
     * Initializes the Angular application. This method should be
     * called before initializing any other app component
     */
    function initAngularApp() {
        Main.appModule = angular.module(Main.appModuleName, ['ngRoute', 'ui.bootstrap']);
    }
    Main.initAngularApp = initAngularApp;
})(Main || (Main = {}));
Main.initAngularApp();
//# sourceMappingURL=root.ng.js.map