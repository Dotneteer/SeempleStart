module Core {
    // ------------------------------------------------------------------------
    // This variable holds the name of this module
    // ------------------------------------------------------------------------
    export var appModuleName: string = 'Core';

    // ------------------------------------------------------------------------
    // This variable represents this module for the external world
    // ------------------------------------------------------------------------
    export var appModule: ng.IModule;

    // ------------------------------------------------------------------------
    // Initializes this module. This method should be
    // called before initializing any other module components
    // ------------------------------------------------------------------------
    export function initModule() {
        Core.appModule = angular.module(appModuleName, []);
    }
}

Core.initModule();  