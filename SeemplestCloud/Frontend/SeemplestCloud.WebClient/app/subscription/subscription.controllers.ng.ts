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
    // This class represents an invitation entry view model
    // ------------------------------------------------------------------------
    export class UserInvitationVm {
        id: number;
        userId: string;
        subscriptionId: number;
        invitedEmail: string;
        invitedUserName: string;
        invitationCode: string;
        expirationDateUtc: Date;
        state: string;
        type: string;
        createdUtc: Date;
        lastModifiedUtc: Date;

        constructor(d?: UserInvitationVm) {
            d = d || <UserInvitationVm>{};
            this.id = d.id;
            this.userId = d.userId;
            this.subscriptionId = d.subscriptionId;
            this.invitedEmail = d.invitedEmail;
            this.invitedUserName = d.invitedUserName;
            this.invitationCode = d.invitationCode;
            this.expirationDateUtc = d.expirationDateUtc;
            this.state = d.state;
            this.type = d.type;
            this.createdUtc = d.createdUtc;
            this.lastModifiedUtc = d.lastModifiedUtc;
        }
    }

    // ------------------------------------------------------------------------
    // Defines the scope of the UserInvitation controller
    // ------------------------------------------------------------------------
    export interface IUserInvitationCtrlScope extends Core.ISimpleSearch, ng.IScope {
        invitations: UserInvitationVm[];
        openInvitationEditor: () => void;
        refreshData: () => void;
        revokeInvitation: () => void;
    }

    // ------------------------------------------------------------------------
    // This controller manages the user invitation view
    // ------------------------------------------------------------------------
    export class UserInvitationCtrl {
        public static $inject = ['$scope', '$modal']

        constructor(
            $scope: IUserInvitationCtrlScope,
            $modal: ng.ui.bootstrap.IModalService) {

            $scope.refreshData = () => {
                $scope.invitations = [
                    new UserInvitationVm({
                        id: 1,
                        userId: '1',
                        invitedEmail: 'dotneteer@hotmail.com',
                        invitedUserName: 'dotneteer',
                        createdUtc: new Date(),
                        expirationDateUtc: moment.utc(new Date(2015, 1, 1)).toDate(),
                        state: 'Sent',
                        type: 'User',
                        invitationCode: '',
                        lastModifiedUtc: null,
                        subscriptionId: null
                    }),
                    new UserInvitationVm({
                        id: 2,
                        userId: '1',
                        invitedEmail: 'vsxguy@gmail.com',
                        invitedUserName: 'vsxguy',
                        createdUtc: new Date(),
                        expirationDateUtc: moment.utc(new Date(2015, 1, 1)).toDate(),
                        state: 'Read',
                        type: 'User',
                        invitationCode: '',
                        lastModifiedUtc: null,
                        subscriptionId: null
                    }),
                    new UserInvitationVm({
                        id: 3,
                        userId: '2',
                        invitedEmail: 'inovak@grepton.hu',
                        invitedUserName: 'Novák István',
                        createdUtc: new Date(),
                        expirationDateUtc: moment.utc(new Date(2015, 1, 1)).toDate(),
                        state: 'Activated',
                        type: 'User',
                        invitationCode: '',
                        lastModifiedUtc: null,
                        subscriptionId: null
                    })
                ];
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
                    windowClass: 'popupPosition'
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
    class EditInvitationCtrl
    extends Core.PopupCtrlBase<UserInvitationVm> {
        public static $inject = ['$scope', '$modalInstance', 'subscriptionApi'];

        constructor(
            $scope: IEditInvitationCtrlScope,
            $modalInstance: ng.ui.bootstrap.IModalServiceInstance,
            api: ISubscriptionApi) {

            super($scope, $modalInstance);

            $scope.onOk = () => {
                api.inviteUser(new InviteUserDto($scope.model.invitedUserName, $scope.model.invitedEmail))
                    .onSuccess(() => { $modalInstance.close(); })
                    .reject(() => { $scope.disableOk = false; })
                    .go();
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