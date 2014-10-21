var Core;
(function (Core) {
    // ------------------------------------------------------------------------
    // Initializes the angular objects defined in this module fragment
    // ------------------------------------------------------------------------
    function initCurrentSpot() {
        Core.appModule.controller('MainCtrl', MainCtrl).factory('currentSpot', [currentSpotService]).directive('currentSpot', currentSpotDirective).directive('activeMenu', ['currentSpot', currentSpotMarkerDirective]);
    }
    Core.initCurrentSpot = initCurrentSpot;

    

    // ------------------------------------------------------------------------
    // This service stores the current spot information
    // ------------------------------------------------------------------------
    function currentSpotService() {
        var currentTitle;
        var activeMenu;
        var languageChoiceEnabled = true;

        return {
            setCurrentSpot: function (title, item) {
                currentTitle = title;
                activeMenu = item;
            },
            disableLanguageChoice: function () {
                languageChoiceEnabled = false;
            },
            getCurrentTitle: function () {
                return currentTitle;
            },
            getActiveMenu: function () {
                return activeMenu;
            },
            isLanguageChoiceEnabled: function () {
                return languageChoiceEnabled;
            }
        };
    }
    Core.currentSpotService = currentSpotService;

    

    // ------------------------------------------------------------------------
    // The controller managing the current spot
    // ------------------------------------------------------------------------
    var CurrentSpotController = (function () {
        function CurrentSpotController($scope, currentSpot) {
            $scope.getCurrentSpot = function () {
                return currentSpot.getCurrentTitle();
            };
        }
        CurrentSpotController.$inject = ['$scope', 'currentSpot'];
        return CurrentSpotController;
    })();
    Core.CurrentSpotController = CurrentSpotController;

    // ------------------------------------------------------------------------
    // The directive that defines how the current spot should be displayed
    // ------------------------------------------------------------------------
    function currentSpotDirective() {
        return {
            restrict: 'E',
            templateUrl: '/app/core/currentSpotTemplate.html',
            controller: CurrentSpotController
        };
    }
    Core.currentSpotDirective = currentSpotDirective;

    // ------------------------------------------------------------------------
    // This directive displays the current menu item ('data-cs-menu' attribute)
    // and the current title ('data-cs-title' attribute) in the 'current-spot'
    // element
    // ------------------------------------------------------------------------
    function currentSpotMarkerDirective(currentSpot) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
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
    Core.currentSpotMarkerDirective = currentSpotMarkerDirective;

    

    // ------------------------------------------------------------------------
    // Implements the controller managing the main screen
    // ------------------------------------------------------------------------
    var MainCtrl = (function () {
        function MainCtrl($scope, currentSpot) {
            $scope.isActive = function (menu) {
                return currentSpot.getActiveMenu() == menu;
            };

            $scope.isLanguageChoiceEnabled = function () {
                return currentSpot.isLanguageChoiceEnabled();
            };
        }
        MainCtrl.$inject = ['$scope', 'currentSpot'];
        return MainCtrl;
    })();
    Core.MainCtrl = MainCtrl;
})(Core || (Core = {}));

Core.initCurrentSpot();
//# sourceMappingURL=currentSpot.ng.js.map
