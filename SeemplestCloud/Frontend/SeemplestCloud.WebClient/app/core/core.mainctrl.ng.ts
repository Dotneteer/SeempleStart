module Core {

    // ------------------------------------------------------------------------
    // Initializes the angular objects defined in this module fragment
    // ------------------------------------------------------------------------
    export function initCurrentSpot() {
        Core.appModule
            .controller('MainCtrl', MainCtrl)
            .factory('currentSpot', [currentSpotService])
            .directive('currentSpot', currentSpotDirective)
            .directive('activeMenu', ['currentSpot', currentSpotMarkerDirective]);
    }

    // ------------------------------------------------------------------------
    // Defines the operations of the current spot service
    // ------------------------------------------------------------------------
    export interface ICurrentSpotService {
        // --- Sets the current spot information
        setCurrentSpot: (title: string, item: string) => void;

        // --- Disables the language choice menu
        disableLanguageChoice: () => void;

        // --- Gets the current title information
        getCurrentTitle: () => string;

        // --- Gets the active menu item's identifier
        getActiveMenu: () => string;

        // --- Gets the flag indicating whether language choice is enable or not
        isLanguageChoiceEnabled: () => boolean;

        // --- Sets the last error message to show
        setLastError: (message: string) => void;

        // --- Gets the last error message to show
        getLastError: () => string;

        // --- Resets the error
        resetError: () => void;

        // --- Gets the flag indicating if there is an error
        hasError: () => boolean;
    }

    // ------------------------------------------------------------------------
    // This service stores the current spot information
    // ------------------------------------------------------------------------
    export function currentSpotService(): ICurrentSpotService {
        var currentTitle: string;
        var activeMenu: string;
        var languageChoiceEnabled = true;
        var lastError: string = null;

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
            },

            setLastError: (message: string) => {
                lastError = message;
            },

            getLastError: () => {
                return lastError;
            },

            resetError: () => {
                lastError = null;
            },

            hasError: () => {
                return !!lastError && lastError != "";
            }
        }
    }

    // ------------------------------------------------------------------------
    // The scope of the controller that gets the current view's title
    // ------------------------------------------------------------------------
    export interface ICurrentSpotScope extends ng.IScope {
        getCurrentSpot: () => string;
    }

    // ------------------------------------------------------------------------
    // The controller managing the current spot
    // ------------------------------------------------------------------------
    export class CurrentSpotController {
        public static $inject = ['$scope', 'currentSpot'];

        constructor($scope: ICurrentSpotScope, currentSpot: ICurrentSpotService) {
            $scope.getCurrentSpot = () => {
                return currentSpot.getCurrentTitle();
            }
        }
    }

    // ------------------------------------------------------------------------
    // The directive that defines how the current spot should be displayed
    // ------------------------------------------------------------------------
    export function currentSpotDirective(): ng.IDirective {
        return {
            restrict: 'E',
            templateUrl: '/app/core/currentSpotTemplate.html',
            controller: CurrentSpotController,
        }
    }

    // ------------------------------------------------------------------------
    // This directive displays the current menu item ('data-cs-menu' attribute) 
    // and the current title ('data-cs-title' attribute) in the 'current-spot' 
    // element
    // ------------------------------------------------------------------------
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

    // ------------------------------------------------------------------------
    // The scope of the controller managing the main screen
    // ------------------------------------------------------------------------
    export interface IMainCtrlScope extends ng.IScope {
        // --- Checkes whether the passed menu identifier represents the active one
        isActive: (menu: string) => boolean;

        // --- Gets the flag indicating if language choice is enabled on UI
        isLanguageChoiceEnabled: () => boolean;

        // --- Gets the current culture information
        getCurrentCulture: () => string;

        // --- Gets the specified service message
        getServiceMessage: (code: string, pars?: {}) => string;

        // --- Gets the string representing the last error
        getLastError: () => string;

        // --- Gets the flag indicating if there is an error
        hasError: () => boolean;
    }

    // ------------------------------------------------------------------------
    // Implements the controller managing the main screen
    // ------------------------------------------------------------------------
    export class MainCtrl {
        public static $inject = ['$scope', 'currentSpot', 'intlnSrv'];

        constructor(
            $scope: IMainCtrlScope,
            currentSpot: ICurrentSpotService,
            intlnSrv: IInternationalizationService) {

            $scope.isActive = (menu: string) => {
                return currentSpot.getActiveMenu() == menu;
            };

            $scope.isLanguageChoiceEnabled = () => {
                return currentSpot.isLanguageChoiceEnabled();
            }

            $scope.getCurrentCulture = () => {
                return intlnSrv.getCurrentCulture();
            }

            $scope.getServiceMessage = (code: string, pars?: {}) => {
                return intlnSrv.getServiceMessage (code, pars);
            };

            $scope.getLastError = () => {
                return currentSpot.getLastError();
            }

            $scope.hasError = () => {
                return currentSpot.hasError();
            }
        }
    }
}

Core.initCurrentSpot();