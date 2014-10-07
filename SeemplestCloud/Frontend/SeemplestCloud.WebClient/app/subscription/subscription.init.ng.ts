module Subscription {
    export var appModuleName: string = 'Subscription';
    export var appModule: ng.IModule;

    /*
     * Initializes this module. This method should be
     * called before initializing any other module components
     */
    export function initModule() {
        appModule = angular.module(appModuleName, []);
        appModule.controller("PackageSelectionCtrl", PackageSelectionCtrl);
    }

    /**
    * The scope of the controller that manages packages
    */
    export interface IPackageSelectionScope extends ng.IScope {
        selectPackage: (string) => void;
        getPackage: () => string;
        isSelected: (string) => boolean;
        model: any;
    }

    /**
    * The controller managing the current spot
    */
    export class PackageSelectionCtrl {
        public static $inject = ['$scope'];

        constructor($scope: IPackageSelectionScope) {
            var packageCode: string;

            $scope.model = {};

            $scope.selectPackage = (code: string) => {
                packageCode = code;
                $scope.model.PackageCode = code;
            }

            $scope.getPackage = () => {
                return packageCode;
            }

            $scope.isSelected = (code: string) => {
                return packageCode == code;
            }
        }
    }
}

Subscription.initModule();   