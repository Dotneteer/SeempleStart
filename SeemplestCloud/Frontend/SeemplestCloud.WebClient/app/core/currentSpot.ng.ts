module Core {
    /*
     * Initializes the components of the main view
     */
    export function initCurrentSpot() {
        Core.appModule
            .controller('CurrentSpotCtrl', CurrentSpotCtrl)
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

        /*
         * Disables the lanugae choice menu
         */
        disableLanguageChoice: () => {};

        /**
         * Gets the current title information
         */
        getCurrentTitle: () => string;

        /**
         * Gets the active menu item's identifier
         */
        getActiveMenu: () => string;

        /*
         * Gets the flag indicating whether language choice is enable or not
         */
        isLanguageChoiceEnabled: () => boolean;
    }

    /**
     * This service stores the current spot information
     */
    export function currentSpotService(): ICurrentSpotService {
        var currentTitle: string;
        var activeMenu: string;
        var languageChoiceEnabled = true;

        return <ICurrentSpotService>{
            setCurrentSpot: (title: string, item: string) => {
                currentTitle = title;
                activeMenu = item;
            },

            disableLanguageChoice: () => {
                languageChoiceEnabled = false;
            },

            getCurrentTitle: () => {
                return currentTitle;
            },

            getActiveMenu: () => {
                return activeMenu;
            },

            isLanguageChoiceEnabled: () => {
                return languageChoiceEnabled;
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
            templateUrl: '/app/core/currentSpotTemplate.html',
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
                var noLanguageChoice = attrs["noLanguageChoice"];
                currentSpot.setCurrentSpot(title, menuId);
                if (!angular.isUndefined(noLanguageChoice)) {
                    currentSpot.disableLanguageChoice();
                }
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
        isLanguageChoiceEnabled: () => boolean;
    }

    /**
     * Implements the controller managing the main screen
     */
    export class CurrentSpotCtrl {
        public static $inject = ['$scope', 'currentSpot'];

        constructor(
            $scope: IMainViewCtrlScope,
            currentSpot: ICurrentSpotService) {

            $scope.isActive = (menu: string) => {
                return currentSpot.getActiveMenu() == menu;
            };

            $scope.isLanguageChoiceEnabled = () => {
                return currentSpot.isLanguageChoiceEnabled();
            }
        }
    }
}

Core.initCurrentSpot();