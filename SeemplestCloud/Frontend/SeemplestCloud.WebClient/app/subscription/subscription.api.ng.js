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
    function initSubscriptionApi() {
        Subscription.appModule.service('subscriptionApi', ['$http', 'currentSpot', SubScriptionApi]);
    }
    Subscription.initSubscriptionApi = initSubscriptionApi;

    // ------------------------------------------------------------------------
    // This class represents an invitation entry view model
    // ------------------------------------------------------------------------
    var UserInvitationVm = (function () {
        function UserInvitationVm() {
        }
        return UserInvitationVm;
    })();
    Subscription.UserInvitationVm = UserInvitationVm;

    // ------------------------------------------------------------------------
    // This DTO represents an invited user
    // ------------------------------------------------------------------------
    var InviteUserDto = (function () {
        function InviteUserDto(invitedUser, invitedEmail) {
            this.invitedUserName = invitedUser;
            this.invitedEmail = invitedEmail;
        }
        return InviteUserDto;
    })();
    Subscription.InviteUserDto = InviteUserDto;

    

    // ------------------------------------------------------------------------
    // This object implements the subscription API
    // ------------------------------------------------------------------------
    var SubScriptionApi = (function (_super) {
        __extends(SubScriptionApi, _super);
        function SubScriptionApi($http, currentSpot) {
            _super.call(this, $http, currentSpot, '../api/subscription');
        }
        SubScriptionApi.prototype.getInvitedUsers = function () {
            return this.request('GET', this.url('invitations'));
        };

        SubScriptionApi.prototype.inviteUser = function (userInfo) {
            return this.request('POST', this.url('inviteUser'), userInfo);
        };
        return SubScriptionApi;
    })(Core.ApiServiceBase);
    Subscription.SubScriptionApi = SubScriptionApi;
})(Subscription || (Subscription = {}));

Subscription.initSubscriptionApi();
//# sourceMappingURL=subscription.api.ng.js.map
