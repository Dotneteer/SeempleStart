var Core;
(function (Core) {
    // ------------------------------------------------------------------------
    // Initializes the angular objects defined in this module fragment
    // ------------------------------------------------------------------------
    function initCurrentSpot() {
        Core.appModule.controller('MainCtrl', MainCtrl).service('currentSpot', [CurrentSpotService]).directive('currentSpot', currentSpotDirective).directive('activeMenu', ['currentSpot', currentSpotMarkerDirective]);
    }
    Core.initCurrentSpot = initCurrentSpot;

    

    // ------------------------------------------------------------------------
    // This service stores the current spot information
    // ------------------------------------------------------------------------
    var CurrentSpotService = (function () {
        function CurrentSpotService() {
            var _this = this;
            this.languageChoiceEnabled = true;
            this.errorCode = null;
            this.errorMessage = null;
            this.errorObject = null;
            this.setCurrentSpot = function (title, item) {
                _this.currentTitle = title;
                _this.activeMenu = item;
            };
            this.disableLanguageChoice = function () {
                _this.languageChoiceEnabled = false;
            };
            this.getCurrentTitle = function () {
                return _this.currentTitle;
            };
            this.getActiveMenu = function () {
                return _this.activeMenu;
            };
            this.isLanguageChoiceEnabled = function () {
                return _this.languageChoiceEnabled;
            };
            this.setError = function (reasonCode, message) {
                _this.errorCode = reasonCode;
                _this.errorMessage = message;
            };
            this.getErrorCode = function () {
                return _this.errorCode;
            };
            this.getErrorMessage = function () {
                return _this.errorMessage;
            };
            this.getErrorObject = function () {
                return _this.errorObject;
            };
            this.resetError = function () {
                _this.errorCode = null;
            };
            this.hasError = function () {
                return _this.errorCode != null;
            };
        }
        return CurrentSpotService;
    })();
    Core.CurrentSpotService = CurrentSpotService;

    

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
        function MainCtrl($scope, currentSpot, intlnSrv) {
            $scope.isActive = function (menu) {
                return currentSpot.getActiveMenu() == menu;
            };

            $scope.isLanguageChoiceEnabled = function () {
                return currentSpot.isLanguageChoiceEnabled();
            };

            $scope.getCurrentCulture = function () {
                return intlnSrv.getCurrentCulture();
            };

            $scope.getServiceMessage = function (code, pars) {
                return intlnSrv.getServiceMessage(code, pars);
            };

            $scope.getErrorMessage = function () {
                if (currentSpot.hasError()) {
                    return $scope.getServiceMessage(currentSpot.getErrorCode(), currentSpot.getErrorObject());
                } else {
                    return "";
                }
            };

            $scope.hasError = function () {
                return currentSpot.hasError();
            };
        }
        MainCtrl.$inject = ['$scope', 'currentSpot', 'intlnSrv'];
        return MainCtrl;
    })();
    Core.MainCtrl = MainCtrl;
})(Core || (Core = {}));

Core.initCurrentSpot();
//# sourceMappingURL=core.mainctrl.ng.js.map
