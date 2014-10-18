module Subscription {

    // ------------------------------------------------------------------------
    // Initializes the angular objects defined in this module fragment
    // ------------------------------------------------------------------------
    export function initControllers() {
        appModule
            .controller("UserInvitationCtrl", UserInvitationCtrl)
            .controller("PackageSelectionCtrl", PackageSelectionCtrl);
    }

    // ------------------------------------------------------------------------
    // Defines the scope of the UserInvitation controller
    // ------------------------------------------------------------------------
    export interface IUserInvitationCtrlScope extends Core.ISimpleSearch,
        ng.ui.bootstrap.IModalScope {
        invitations: UserInvitationVm[];
        openInvitationEditor: () => void;
        refreshData: () => void;
        revokeInvitation: () => void;
    }

    // ------------------------------------------------------------------------
    // This controller manages the user invitation view
    // ------------------------------------------------------------------------
    export class UserInvitationCtrl {
        public static $inject = ['$scope', '$modal', 'subscriptionApi']

        constructor(
            $scope: IUserInvitationCtrlScope,
            $modal: ng.ui.bootstrap.IModalService,
            api: ISubscriptionApi) {

            $scope.refreshData = () => {
                api.getInvitedUsers()
                    .onSuccess((response: Core.ResponseData<UserInvitationVm[]>) => {
                        $scope.invitations = response.data;
                    });
            };

            $scope.clearSearchKey = () => {
                $scope.searchKey = '';
            }

            $scope.openInvitationEditor = () => {
                var modalInstance = $modal.open({
                    templateUrl: 'newInvitation',
                    backdrop: 'static',
                    controller: EditInvitationCtrl,
                    size: 'md',
                    windowClass: 'popupPosition',
                    scope: $scope
            });

                modalInstance.result.then(
                    () => {
                        $scope.refreshData();
                    },
                    (dismissReason) => {
                        if (dismissReason == 'deleted') {
                            $scope.refreshData();
                        }
                    });
            }

            $scope.revokeInvitation = () => { }

            $scope.refreshData();
        }
    }

    // ------------------------------------------------------------------------
    // Defines the scope of the user invitation entry popup controller
    // ------------------------------------------------------------------------
    interface IEditInvitationCtrlScope
    extends Core.IPopupCtrlScopeBase<UserInvitationVm> {
        removeInvitation: () => void;
    }

    // ------------------------------------------------------------------------
    // This controller manages the user invitation entry popup
    // ------------------------------------------------------------------------
    class EditInvitationCtrl extends Core.PopupCtrlBase<UserInvitationVm> {
        public static $inject = ['$scope', '$modalInstance', 'subscriptionApi'];

        constructor(
            $scope: IEditInvitationCtrlScope,
            $modalInstance: ng.ui.bootstrap.IModalServiceInstance,
            api: ISubscriptionApi) {

            super($scope, $modalInstance);

            $scope.onOk = () => {
                api.inviteUser(new InviteUserDto($scope.model.invitedUserName, $scope.model.invitedEmail))
                    .onSuccess(() => { $modalInstance.close(); })
                    .conclude(() => { $scope.disableOk = false; });
                return false;
            };

            $scope.removeInvitation = () => {
                // --- Use the API
            }
        }
    }

    // ------------------------------------------------------------------------
    // The scope of the controller that manages packages
    // ------------------------------------------------------------------------
    export interface IPackageSelectionScope extends ng.IScope {
        selectPackage: (string) => void;
        getPackage: () => string;
        isSelected: (string) => boolean;
        model: any;
        message: string;
        result: string;
        result2: string;
    }

    // ------------------------------------------------------------------------
    // The controller managing the current spot
    // ------------------------------------------------------------------------
    export class PackageSelectionCtrl {
        public static $inject = ['$scope', 'subscriptionApi'];

        constructor($scope: IPackageSelectionScope, api: ISubscriptionApi) {
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

Subscription.initControllers();   