module Subscription {
    export var appModuleName: string = 'Subscription';
    export var appModule: ng.IModule;

    export function initModule() {
        appModule = angular.module(appModuleName, []);
    }
}

Subscription.initModule();   