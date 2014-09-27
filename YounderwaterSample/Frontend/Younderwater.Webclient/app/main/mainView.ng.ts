module Main {
    /*
     * Initializes the components of the main view
     */
    export function initMainView() {
        Main.appModule
            .controller('MainViewCtrl', MainViewCtrl)
            .factory('currentSpot', [currentSpotService])
            .directive('currentSpot', currentSpotDirective)
            .directive('activeMenu', ['currentSpot', currentSpotMarkerDirective]);
    }

    /**
    *  Defines the operations of the current spot service
    */
    export interface ICurrentSpotService {
        // --- Az aktuális funkció címének és a hozzá tartozó aktív menü 
        // --- azonosítójának beállítása
        /**
         * Sets the current spot information
         * @param title The title to display as the name of the current spot
         * @param item The menu identifier of the active element
         */
        setCurrentSpot: (title: string, item: string) => {};

        /**
         * Gets the current title information
         */
        getCurrentTitle: () => string;

        /**
         * Gets the active menu item's identifier
         */
        getActiveMenu: () => string;
    }

    /**
     * This service stores the current spot information
     */
    export function currentSpotService(): ICurrentSpotService {
        var currentTitle: string;
        var activeMenu: string;

        return <ICurrentSpotService>{
            setCurrentSpot: (title: string, item: string) => {
                currentTitle = title;
                activeMenu = item;
            },

            getCurrentTitle: () => {
                return currentTitle;
            },

            getActiveMenu: () => {
                return activeMenu;
            }
        }
    }

    /**
     * The scope of the controller that gets the current view's title
    */
    export interface ICurrentSpotScope extends ng.IScope {
        getCurrentSpot: () => string;
    }

    /**
     * The controller managing the current spot
     */
    export class CurrentSpotController {
        public static $inject = ['$scope', 'currentSpot'];

        constructor($scope: ICurrentSpotScope, currentSpot: ICurrentSpotService) {
            $scope.getCurrentSpot = () => {
                return currentSpot.getCurrentTitle();
            }
        }
    }

    /*
     * The directive that defines how the current spot should be displayed
     */
    export function currentSpotDirective(): ng.IDirective {
        return {
            restrict: 'E',
            templateUrl: '/app/main/currentSpotTemplate.html',
            controller: CurrentSpotController,
        }
    }

    /*
     * This directive displays the current menu item ('data-cs-menu' attribute) and
     * the current title ('data-cs-title' attribute) in the 'current-spot' element
     */
    export function currentSpotMarkerDirective(currentSpot: ICurrentSpotService): ng.IDirective {

        return {
            restrict: 'A',
            link: (scope, element, attrs) => {
                var title = attrs["activeTitle"];
                var menuId = attrs["activeMenu"];
                currentSpot.setCurrentSpot(title, menuId);
            }
        };
    }

    /**
     * The scope of the controller managing the main screen
    */
    export interface IMainViewCtrlScope extends ng.IScope {
        /**
         * Checkes whether the passed menu identifier represents the active one
         * @param menu
         */
        isActive: (menu: string) => boolean;
    }

    /**
     * Implements the controller managing the main screen
     */
    export class MainViewCtrl {
        public static $inject = ['$scope', 'currentSpot'];

        constructor(
            $scope: IMainViewCtrlScope,
            currentSpot: ICurrentSpotService) {

            $scope.isActive = (menu: string) => {
                return currentSpot.getActiveMenu() == menu;
            };
        }
    }
}

Main.initAngularApp();
Main.initMainView();