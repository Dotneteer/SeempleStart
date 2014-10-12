module Subscription {
    export var appModuleName: string = 'Subscription';
    export var appModule: ng.IModule;

    /*
     * Initializes this module. This method should be
     * called before initializing any other module components
     */
    export function initModule() {
        appModule = angular.module(appModuleName, []);
        appModule
            .controller("UserInvitationCtrl", UserInvitationCtrl)
            .controller("PackageSelectionCtrl", PackageSelectionCtrl);
    }

    /*
     * This class represents an invitation entry
     */
    export class UserInvitationDto {
        id: number;
        userId: string;
        invitedEmail: string;
        invitedUserName: string;
        createdUtc: Date;
        expirationDateUtc: Date;
        state: string;

        constructor(d?: UserInvitationDto) {
            d = d || <UserInvitationDto>{};
            this.id = d.id;
            this.userId = d.userId;
            this.invitedEmail = d.invitedEmail;
            this.invitedUserName = d.invitedUserName;
            this.createdUtc = d.createdUtc;
            this.expirationDateUtc = d.expirationDateUtc;
            this.state = d.state;
        }
    }

    /*
     * Defines the scope of the UserInvitation controller
     */
    export interface IUserInvitationCtrlScope extends Core.ISimpleSearch, ng.IScope {
        invitations: UserInvitationDto[];
        openInvitationEditor: (invitation: UserInvitationDto) => void;
        refreshData: () => void;
        revokeInvitation: () => void;
    }

    /*
     * This controller manages the user invitation view
     */
    export class UserInvitationCtrl {
        public static $inject = ['$scope', '$modal']

        constructor(
            $scope: IUserInvitationCtrlScope,
            $modal: ng.ui.bootstrap.IModalService) {

            $scope.refreshData = () => {
                $scope.invitations = [
                    new UserInvitationDto({
                        id: 1,
                        userId: '1',
                        invitedEmail: 'dotneteer@hotmail.com',
                        invitedUserName: 'dotneteer',
                        createdUtc: new Date(),
                        expirationDateUtc: moment.utc(new Date(2015, 1, 1)).toDate(),
                        state: 'Sent'
                    }),
                    new UserInvitationDto({
                        id: 2,
                        userId: '1',
                        invitedEmail: 'vsxguy@gmail.com',
                        invitedUserName: 'vsxguy',
                        createdUtc: new Date(),
                        expirationDateUtc: moment.utc(new Date(2015, 1, 1)).toDate(),
                        state: 'Read'
                    }),
                    new UserInvitationDto({
                        id: 3,
                        userId: '2',
                        invitedEmail: 'inovak@grepton.hu',
                        invitedUserName: 'Novák István',
                        createdUtc: new Date(),
                        expirationDateUtc: moment.utc(new Date(2015, 1, 1)).toDate(),
                        state: 'Activated'
                    })
                ];
            };

            $scope.clearSearchKey = () => {
                $scope.searchKey = '';
            }

            $scope.openInvitationEditor = (invitation: UserInvitationDto) => {
                var invitationBackup = angular.copy(invitation);

                var modalInstance = $modal.open({
                    templateUrl: 'newInvitation',
                    backdrop: 'static',
                    controller: EditInvitationCtrl,
                    size: 'md',
                    windowClass: 'popupPosition',
                    resolve: {
                        invitation: () => { return invitation; }
                    }
                });

                modalInstance.result.then(
                    (data) => {
                        $scope.refreshData();
                    },
                    (dismissReason) => {
                        if (dismissReason == 'deleted') {
                            $scope.refreshData();
                        }
                    });
            }

            $scope.revokeInvitation = () => {}
   
            $scope.refreshData();
        }
    }

    /*
     * Defines the scope of the user invitation entry popup controller
     */
    interface IEditInvitationCtrlScope extends ng.IScope {
        invitation: UserInvitationDto;
        ok: () => void;
        removeInvitation: () => void;
        cancel: () => void;
    }

    /*
     * This controller manages the dive log entry popup
     */
    class EditInvitationCtrl {
        public static $inject = ['$scope', '$modalInstance', 'invitation'];

        constructor(
            $scope: IEditInvitationCtrlScope,
            $modalInstance: ng.ui.bootstrap.IModalServiceInstance,
            invitation: UserInvitationDto) {
            $scope.invitation = invitation || new UserInvitationDto();

            $scope.ok = () => {
                    // Use the api
                    //api.DiveLog.registerDiveLogEntry($scope.dive).success(() => {
                    //    $modalInstance.close();
                    //});
            };

            $scope.removeInvitation = () => {
                // use the api
                //api.DiveLog.removeDiveLogEntry($scope.dive.id).success(() => {
                //    $modalInstance.dismiss('deleted');
                //});
            }
            $scope.cancel = () => {
                $modalInstance.dismiss('cancel');
            };
        }
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