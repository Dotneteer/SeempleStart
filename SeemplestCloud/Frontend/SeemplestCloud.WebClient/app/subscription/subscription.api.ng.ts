module Subscription {

    // ------------------------------------------------------------------------
    // Initializes the angular objects defined in this module fragment
    // ------------------------------------------------------------------------
    export function initSubscriptionApi() {
        Subscription.appModule
            .service('subscriptionApi', ['$http', 'currentSpot', SubScriptionApi]);
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
        expirationDateUtc: Date;
        state: string;
        type: string;
    }

    // ------------------------------------------------------------------------
    // This DTO represents an invited user
    // ------------------------------------------------------------------------
    export class InviteUserDto {
        invitedUserName: string;
        invitedEmail: string;

        constructor(invitedUser: string, invitedEmail: string) {
            this.invitedUserName = invitedUser;
            this.invitedEmail = invitedEmail;
        }
    }

    // ------------------------------------------------------------------------
    // This interface represents the WebAPI managing subscriptions
    // ------------------------------------------------------------------------
    export interface ISubscriptionApi {
        getInvitedUsers(): Core.IBusinessPromise<UserInvitationVm[]>
        inviteUser(userInfo: InviteUserDto): Core.IBusinessPromise<any>;
    }
    
    // ------------------------------------------------------------------------
    // This object implements the subscription API
    // ------------------------------------------------------------------------
    export class SubScriptionApi extends Core.ApiServiceBase implements ISubscriptionApi {

        constructor($http: ng.IHttpService, currentSpot: Core.ICurrentSpotService) {
            super($http, currentSpot, '../api/subscription');
        }

        getInvitedUsers() {
            return this.request('GET', this.url('invitations'));
        }

        inviteUser(userInfo: InviteUserDto) {
            return this.request('POST', this.url('inviteUser'), userInfo);
        }
    }
}

Subscription.initSubscriptionApi();