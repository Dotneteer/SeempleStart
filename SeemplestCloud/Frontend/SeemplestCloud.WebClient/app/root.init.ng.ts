module Main {

    // ------------------------------------------------------------------------
    // This variable holds the name of this application
    // ------------------------------------------------------------------------
    export var appModuleName: string = 'SeemplestCloud';

    // ------------------------------------------------------------------------
    // This variable represents this application for the external world
    // ------------------------------------------------------------------------
    export var appModule: ng.IModule;

    // ------------------------------------------------------------------------
    // Initializes the Angular application. This method should be
    // called before initializing any other app component
    // ------------------------------------------------------------------------
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