var Main;
(function (Main) {
    /*
     * Initializes the components of the main view
     */
    function initMainView() {
        Main.appModule
            .controller('MainViewCtrl', MainViewCtrl)
            .factory('currentSpot', [currentSpotService])
            .directive('currentSpot', currentSpotDirective)
            .directive('activeMenu', ['currentSpot', currentSpotMarkerDirective]);
    }
    Main.initMainView = initMainView;
    /**
     * This service stores the current spot information
     */
    function currentSpotService() {
        var currentTitle;
        var activeMenu;
        return {
            setCurrentSpot: function (title, item) {
                currentTitle = title;
                activeMenu = item;
            },
            getCurrentTitle: function () {
                return currentTitle;
            },
            getActiveMenu: function () {
                return activeMenu;
            }
        };
    }
    Main.currentSpotService = currentSpotService;
    /**
     * The controller managing the current spot
     */
    var CurrentSpotController = (function () {
        function CurrentSpotController($scope, currentSpot) {
            $scope.getCurrentSpot = function () {
                return currentSpot.getCurrentTitle();
            };
        }
        CurrentSpotController.$inject = ['$scope', 'currentSpot'];
        return CurrentSpotController;
    }());
    Main.CurrentSpotController = CurrentSpotController;
    /*
     * The directive that defines how the current spot should be displayed
     */
    function currentSpotDirective() {
        return {
            restrict: 'E',
            templateUrl: '/app/main/currentSpotTemplate.html',
            controller: CurrentSpotController,
        };
    }
    Main.currentSpotDirective = currentSpotDirective;
    /*
     * This directive displays the current menu item ('data-cs-menu' attribute) and
     * the current title ('data-cs-title' attribute) in the 'current-spot' element
     */
    function currentSpotMarkerDirective(currentSpot) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var title = attrs["activeTitle"];
                var menuId = attrs["activeMenu"];
                currentSpot.setCurrentSpot(title, menuId);
            }
        };
    }
    Main.currentSpotMarkerDirective = currentSpotMarkerDirective;
    /**
     * Implements the controller managing the main screen
     */
    var MainViewCtrl = (function () {
        function MainViewCtrl($scope, currentSpot) {
            $scope.isActive = function (menu) {
                return currentSpot.getActiveMenu() == menu;
            };
        }
        MainViewCtrl.$inject = ['$scope', 'currentSpot'];
        return MainViewCtrl;
    }());
    Main.MainViewCtrl = MainViewCtrl;
})(Main || (Main = {}));
Main.initAngularApp();
Main.initMainView();
//# sourceMappingURL=mainView.ng.js.map