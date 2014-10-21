var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var Subscription;
(function (Subscription) {
    // ------------------------------------------------------------------------
    // Initializes the angular objects defined in this module fragment
    // ------------------------------------------------------------------------
    function initControllers() {
        Subscription.appModule.controller("UserInvitationCtrl", UserInvitationCtrl).controller("PackageSelectionCtrl", PackageSelectionCtrl);
    }
    Subscription.initControllers = initControllers;

    

    // ------------------------------------------------------------------------
    // This controller manages the user invitation view
    // ------------------------------------------------------------------------
    var UserInvitationCtrl = (function () {
        function UserInvitationCtrl($scope, $modal, api) {
            $scope.refreshData = function () {
                api.getInvitedUsers().onSuccess(function (response) {
                    $scope.invitations = response.data;
                });
            };

            $scope.clearSearchKey = function () {
                $scope.searchKey = '';
            };

            $scope.openInvitationEditor = function () {
                var modalInstance = $modal.open({
                    templateUrl: 'newInvitation',
                    backdrop: 'static',
                    controller: EditInvitationCtrl,
                    size: 'md',
                    windowClass: 'popupPosition',
                    scope: $scope
                });

                modalInstance.result.then(function () {
                    $scope.refreshData();
                }, function (dismissReason) {
                    if (dismissReason == 'deleted') {
                        $scope.refreshData();
                    }
                });
            };

            $scope.revokeInvitation = function () {
            };

            $scope.refreshData();
        }
        UserInvitationCtrl.$inject = ['$scope', '$modal', 'subscriptionApi'];
        return UserInvitationCtrl;
    })();
    Subscription.UserInvitationCtrl = UserInvitationCtrl;

    

    // ------------------------------------------------------------------------
    // This controller manages the user invitation entry popup
    // ------------------------------------------------------------------------
    var EditInvitationCtrl = (function (_super) {
        __extends(EditInvitationCtrl, _super);
        function EditInvitationCtrl($scope, $modalInstance, api) {
            _super.call(this, $scope, $modalInstance);

            $scope.onOk = function () {
                api.inviteUser(new Subscription.InviteUserDto($scope.model.invitedUserName, $scope.model.invitedEmail)).onSuccess(function () {
                    $modalInstance.close();
                }).conclude(function () {
                    $scope.disableOk = false;
                });
                return false;
            };

            $scope.removeInvitation = function () {
                // --- Use the API
            };
        }
        EditInvitationCtrl.$inject = ['$scope', '$modalInstance', 'subscriptionApi'];
        return EditInvitationCtrl;
    })(Core.PopupCtrlBase);

    

    // ------------------------------------------------------------------------
    // The controller managing the current spot
    // ------------------------------------------------------------------------
    var PackageSelectionCtrl = (function () {
        function PackageSelectionCtrl($scope, api) {
            var packageCode;

            $scope.model = {};

            $scope.selectPackage = function (code) {
                packageCode = code;
                $scope.model.PackageCode = code;
            };

            $scope.getPackage = function () {
                return packageCode;
            };

            $scope.isSelected = function (code) {
                return packageCode == code;
            };
        }
        PackageSelectionCtrl.$inject = ['$scope', 'subscriptionApi'];
        return PackageSelectionCtrl;
    })();
    Subscription.PackageSelectionCtrl = PackageSelectionCtrl;
})(Subscription || (Subscription = {}));

Subscription.initControllers();
//# sourceMappingURL=subscription.controllers.ng.js.map
