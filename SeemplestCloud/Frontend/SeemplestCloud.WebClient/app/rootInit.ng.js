﻿var Main;
(function (Main) {
    Main.appModuleName = 'SeemplestCloud';
    Main.appModule;

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
//# sourceMappingURL=rootInit.ng.js.map
