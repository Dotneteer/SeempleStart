module Dive {
    /*
     * Initializes the components of the dive services
     */
    export function initAccountComponents() {
        Main.appModule
            .filter('diveSearch', [diveSearch])
            .controller('MyDivesCtrl', MyDivesCtrl);
    }

    /*
     * This class represents a dive log entry
     */
    export class DiveLogEntryDto {
        id: number;
        date: Date;
        diveSite: string;
        location: string;
        maxDepth: number;
        bottomTime: number;
        comment: string;

        constructor(d?: DiveLogEntryDto) {
            d = d || <DiveLogEntryDto>{};
            this.id = d.id;
            this.date = d.date;
            this.diveSite = d.diveSite;
            this.location = d.location;
            this.maxDepth = d.maxDepth;
            this.bottomTime = d.bottomTime;
            this.comment = d.comment;
        }
    }

    /*
     * This class implements a filter that can be used to search for dives according the
     * name and location of the dive site
     */
    export function diveSearch() {
    return (data: DiveLogEntryDto[], searchKey: string) => {
            if (angular.isArray(data) && angular.isString(searchKey)) {
                var filtered = Array<DiveLogEntryDto>();
                var key = searchKey.toLocaleLowerCase();
                for (var i = 0; i < data.length; i++) {
                    if (data[i].diveSite.toLocaleLowerCase().indexOf(key) >= 0 ||
                        data[i].location.toLocaleLowerCase().indexOf(key) >= 0) {
                        filtered.push(data[i]);
                    }
                }
                return filtered;
            } else {
                return data;
            }
        }
    }

    /*
     * Defines the scope of the MyDives controller
     */
    export interface IMyDivesCtrlScope extends Core.ISimpleSearch, ng.IScope {
        dives: DiveLogEntryDto[];
        openDiveEditor: (dive: DiveLogEntryDto, isEdit: boolean) => void;
        refreshData: () => void;
    }

    /*
     * This controller manages the dive log screen
     */
    export class MyDivesCtrl {
        public static $inject = ['$scope', '$modal', 'ywapi']

        constructor(
            $scope: IMyDivesCtrlScope,
            $modal: ng.ui.bootstrap.IModalService,
            api: YwApi.YwApiService) {

            $scope.refreshData = () => {
                api.DiveLog.getAllDivesOfUser().success(data => {
                    $scope.dives = data;
                });
            }

            $scope.clearSearchKey = () => {
                $scope.searchKey = '';
            }

            $scope.openDiveEditor = (dive: DiveLogEntryDto, isEdit: boolean) => {
                var diveBackup = angular.copy(dive);

                var modalInstance = $modal.open({
                    templateUrl: 'editDive',
                    backdrop: 'static',
                    controller: EditDiveCtrl,
                    size: 'lg',
                    windowClass: 'popupPosition',
                    resolve: {
                        dive: () => { return dive; },
                        isEdit: () => { return isEdit; }
                    }
                });

                modalInstance.result.then(
                (data) => {
                    if (!isEdit) {
                        $scope.refreshData();
                    }
                },
                (dismissReason) => {
                    if (dismissReason == 'deleted') {
                        $scope.refreshData();
                    }
                    if (dismissReason == 'cancel') {
                        $scope.dives[$scope.dives.indexOf($scope.dives.filter(x => x.id == dive.id)[0])] = angular.copy(diveBackup);
                    }
                });
            }

            $scope.refreshData();
        }
    }

    /*
     * Defines the scope of the dive log entry popup controller
     */
    interface IEditDiveCtrlScope extends ng.IScope {
        dive: DiveLogEntryDto;
        isEdit: boolean;
        ok: () => void;
        removeDive: () => void;
        cancel: () => void;
        datePopupOpen: boolean;
        openDatePopup: (any) => void;
    }

    /*
     * This controller manages the dive log entry popup
     */
    class EditDiveCtrl {
        public static $inject = ['$scope', '$modalInstance', 'dive', 'isEdit', 'ywapi'];

        constructor(
            $scope: IEditDiveCtrlScope,
            $modalInstance: ng.ui.bootstrap.IModalServiceInstance,
            dive: DiveLogEntryDto,
            isEdit: boolean,
            api: YwApi.YwApiService) {
            $scope.dive = dive || new DiveLogEntryDto();
            $scope.isEdit = isEdit;
            $scope.datePopupOpen = false;

            $scope.openDatePopup = ($event: any) => {
                $event.preventDefault();
                $event.stopPropagation();
                $scope.datePopupOpen = true;
            };

            $scope.ok = () => {
                if (isEdit) {
                    api.DiveLog.modifyDiveLogEntry($scope.dive).success(() => {
                        $modalInstance.close();
                    });
                } else {
                    api.DiveLog.registerDiveLogEntry($scope.dive).success(() => {
                        $modalInstance.close();
                    });
                }
            };

            $scope.removeDive = () => {
                api.DiveLog.removeDiveLogEntry($scope.dive.id).success(() => {
                    $modalInstance.dismiss('deleted');
                });
            }
            $scope.cancel = () => {
                $modalInstance.dismiss('cancel');
            };
        }
    }
}

Dive.initAccountComponents();