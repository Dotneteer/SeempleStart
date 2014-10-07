module Core {
    export var appModuleName: string = 'Core';
    export var appModule: ng.IModule;

    /*
     * Initializes this module. This method should be
     * called before initializing any other module components
     */
    export function initModule() {
        Core.appModule = angular.module(appModuleName, []);
    }
}

Core.initModule();  