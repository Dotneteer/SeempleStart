module Subscription {

    // ------------------------------------------------------------------------
    // This variable holds the name of this module
    // ------------------------------------------------------------------------
    export var appModuleName: string = 'Subscription';

    // ------------------------------------------------------------------------
    // This variable represents this module for the external world
    // ------------------------------------------------------------------------
    export var appModule: ng.IModule;

    // ------------------------------------------------------------------------
    // Initializes this module. This method should be
    // called before initializing any other module components
    // ------------------------------------------------------------------------
    export function initModule() {
        appModule = angular.module(appModuleName, []);
    }
}

Subscription.initModule();   