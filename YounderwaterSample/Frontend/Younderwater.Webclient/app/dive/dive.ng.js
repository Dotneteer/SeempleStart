var Dive;
(function (Dive) {
    /*
     * Initializes the components of the dive services
     */
    function initAccountComponents() {
        Main.appModule
            .filter('diveSearch', [diveSearch])
            .controller('MyDivesCtrl', MyDivesCtrl);
    }
    Dive.initAccountComponents = initAccountComponents;
    /*
     * This class represents a dive log entry
     */
    var DiveLogEntryDto = (function () {
        function DiveLogEntryDto(d) {
            d = d || {};
            this.id = d.id;
            this.date = d.date;
            this.diveSite = d.diveSite;
            this.location = d.location;
            this.maxDepth = d.maxDepth;
            this.bottomTime = d.bottomTime;
            this.comment = d.comment;
        }
        return DiveLogEntryDto;
    }());
    Dive.DiveLogEntryDto = DiveLogEntryDto;
    /*
     * This class implements a filter that can be used to search for dives according the
     * name and location of the dive site
     */
    function diveSearch() {
        return function (data, searchKey) {
            if (angular.isArray(data) && angular.isString(searchKey)) {
                var filtered = Array();
                var key = searchKey.toLocaleLowerCase();
                for (var i = 0; i < data.length; i++) {
                    if (data[i].diveSite.toLocaleLowerCase().indexOf(key) >= 0 ||
                        data[i].location.toLocaleLowerCase().indexOf(key) >= 0) {
                        filtered.push(data[i]);
                    }
                }
                return filtered;
            }
            else {
                return data;
            }
        };
    }
    Dive.diveSearch = diveSearch;
    /*
     * This controller manages the dive log screen
     */
    var MyDivesCtrl = (function () {
        function MyDivesCtrl($scope, $modal, api) {
            $scope.refreshData = function () {
                api.DiveLog.getAllDivesOfUser().success(function (data) {
                    $scope.dives = data;
                });
            };
            $scope.clearSearchKey = function () {
                $scope.searchKey = '';
            };
            $scope.openDiveEditor = function (dive, isEdit) {
                var diveBackup = angular.copy(dive);
                var modalInstance = $modal.open({
                    templateUrl: 'editDive',
                    backdrop: 'static',
                    controller: EditDiveCtrl,
                    size: 'lg',
                    windowClass: 'popupPosition',
                    resolve: {
                        dive: function () { return dive; },
                        isEdit: function () { return isEdit; }
                    }
                });
                modalInstance.result.then(function (data) {
                    if (!isEdit) {
                        $scope.refreshData();
                    }
                }, function (dismissReason) {
                    if (dismissReason == 'deleted') {
                        $scope.refreshData();
                    }
                    if (dismissReason == 'cancel') {
                        $scope.dives[$scope.dives.indexOf($scope.dives.filter(function (x) { return x.id == dive.id; })[0])] = angular.copy(diveBackup);
                    }
                });
            };
            $scope.refreshData();
        }
        MyDivesCtrl.$inject = ['$scope', '$modal', 'ywapi'];
        return MyDivesCtrl;
    }());
    Dive.MyDivesCtrl = MyDivesCtrl;
    /*
     * This controller manages the dive log entry popup
     */
    var EditDiveCtrl = (function () {
        function EditDiveCtrl($scope, $modalInstance, dive, isEdit, api) {
            $scope.dive = dive || new DiveLogEntryDto();
            $scope.isEdit = isEdit;
            $scope.datePopupOpen = false;
            $scope.openDatePopup = function ($event) {
                $event.preventDefault();
                $event.stopPropagation();
                $scope.datePopupOpen = true;
            };
            $scope.ok = function () {
                if (isEdit) {
                    api.DiveLog.modifyDiveLogEntry($scope.dive).success(function () {
                        $modalInstance.close();
                    });
                }
                else {
                    api.DiveLog.registerDiveLogEntry($scope.dive).success(function () {
                        $modalInstance.close();
                    });
                }
            };
            $scope.removeDive = function () {
                api.DiveLog.removeDiveLogEntry($scope.dive.id).success(function () {
                    $modalInstance.dismiss('deleted');
                });
            };
            $scope.cancel = function () {
                $modalInstance.dismiss('cancel');
            };
        }
        EditDiveCtrl.$inject = ['$scope', '$modalInstance', 'dive', 'isEdit', 'ywapi'];
        return EditDiveCtrl;
    }());
})(Dive || (Dive = {}));
Dive.initAccountComponents();
//# sourceMappingURL=dive.ng.js.map