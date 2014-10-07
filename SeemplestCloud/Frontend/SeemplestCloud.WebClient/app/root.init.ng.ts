module Main {
    export var appModuleName: string = 'SeemplestCloud';
    export var appModule: ng.IModule;

    /*
     * Initializes the Angular application. This method should be
     * called before initializing any other app component
     */
    export function initAngularApp() {
        Main.appModule = angular.module(appModuleName, [
            // --- 3rd party packages
            'ngRoute',
            'ui.bootstrap',
            
            // --- App-specific packages
            'Core',
            'Subscription'
        ]);
    }
}

Main.initAngularApp(); 