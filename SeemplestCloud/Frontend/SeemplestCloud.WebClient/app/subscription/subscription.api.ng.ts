module Subscription {

    export function initSubscriptionApi() {
        Subscription.appModule
            .service('subscriptionApi', ['$http', SubScriptionApi]);
    }

    export interface ISubscriptionApi {
        getResult(id: number): Core.IBusinessPromise<number>;
        getResult2(id: number): Core.IBusinessPromise<number>;
        getMessage(): Core.IBusinessPromise<string>;
    }

    export class SubScriptionApi extends Core.ApiServiceBase implements ISubscriptionApi {

        constructor($http: ng.IHttpService) {
            super($http, '../api/subscription');
        }

        getResult(id: number) {
            return this.req('GET', this.url('getresult', id));
        }

        getResult2(id: number) {
            return this.req('GET', this.url('getresult2', id));
        }
        getMessage() {
            return this.req('GET', this.url('getmessage'));
        }
    }
}

Subscription.initSubscriptionApi();