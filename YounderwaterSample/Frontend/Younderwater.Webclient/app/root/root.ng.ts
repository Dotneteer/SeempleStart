module Main {
    export var appModuleName: string = 'Younderwater';
    export var appModule: ng.IModule;

    /*
     * Initializes the Angular application. This method should be
     * called before initializing any other app component
     */
    export function initAngularApp() {
        Main.appModule = angular.module(appModuleName, ['ngRoute', 'ui.bootstrap']);
    }
}

Main.initAngularApp();