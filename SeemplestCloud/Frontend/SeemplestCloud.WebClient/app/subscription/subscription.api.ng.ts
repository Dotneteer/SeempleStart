module Subscription {

    // ------------------------------------------------------------------------
    // Initializes the angular objects defined in this module fragment
    // ------------------------------------------------------------------------
    export function initSubscriptionApi() {
        Subscription.appModule
            .service('subscriptionApi', ['$http', SubScriptionApi]);
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
        inviteUser(userInfo: InviteUserDto): Core.IBusinessPromise<any>;
    }
    
    // ------------------------------------------------------------------------
    // This object implements the subscription API
    // ------------------------------------------------------------------------
    export class SubScriptionApi extends Core.ApiServiceBase implements ISubscriptionApi {

        constructor($http: ng.IHttpService) {
            super($http, '../api/subscription');
        }

        inviteUser(userInfo: InviteUserDto) {
            return this.request('POST', this.url('inviteUser'), userInfo);
        }
    }
}

Subscription.initSubscriptionApi();