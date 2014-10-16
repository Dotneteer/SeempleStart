module Core {

    // ------------------------------------------------------------------------
    // Initializes the angular objects defined in this module fragment
    // ------------------------------------------------------------------------
    export function initApis() {
        Subscription.appModule
            .service('intlnApi', ['$http', ""]);
    }

    // ========================================================================
    // Abstract API for calling WebAPI services
    // ========================================================================
    // ------------------------------------------------------------------------
    // This interface describes an infrastructure error
    // ------------------------------------------------------------------------
    export interface IInfrastructureError {
        reasonCode: string;
        isBusiness: boolean;
        message: string;
    }

    // ------------------------------------------------------------------------
    // This class describes a business error
    // ------------------------------------------------------------------------
    export interface IBusinessError extends IInfrastructureError {
        errorObject: any;
    }

    // ------------------------------------------------------------------------
    // This interface defines a business promise callback of type T
    // ------------------------------------------------------------------------
    export interface IBusinessPromiseCallback<T> {
        (data: T, status: number, headers: (headerName: string) => string): void;
    }


    // ------------------------------------------------------------------------
    // --- This class defines a business promise that returns a type of TReturn
    // ------------------------------------------------------------------------
    export interface IBusinessPromise<TReturn> {
        success(callback: IBusinessPromiseCallback<TReturn>): IBusinessPromise<TReturn>;
        error(business: IBusinessPromiseCallback<IBusinessError>,
            unexpected: IBusinessPromiseCallback<IInfrastructureError>): IBusinessPromise<TReturn>;
    }

    // ------------------------------------------------------------------------
    // --- This interface describes a request method
    // ------------------------------------------------------------------------
    export interface IRequestMethod {
        (method: string, url: string, data?: any): IBusinessPromise<any>;
    }

    // ------------------------------------------------------------------------
    // This regex matches with a date of yyyy-MM-ddTHH:mm:ss format
    // ------------------------------------------------------------------------
    var dateTimeStringRegex = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})/;

    // ------------------------------------------------------------------------
    // Converts a JSON string into a date time value
    // ------------------------------------------------------------------------
    function convertDateStringsToDates(input) {
        // Ha a paraméter nem object típusú, visszatérünk.
        if (!input || typeof input !== "object") return;

        for (var key in input) {
            if (!input.hasOwnProperty(key)) continue;

            var value = input[key];
            // Ellenőrzés, hogy az adattag JSON dátum-e
            if (typeof value === "string" && (value.match(dateTimeStringRegex))) {
                input[key] = moment(value).toDate();
            } else if (typeof value === "object") {
                // Ha objektum, akkor rekurzívan ellenőrzünk
                convertDateStringsToDates(value);
            }
        }
    }

    // ------------------------------------------------------------------------
    // This class is intended to be the base class of all API service objects.
    // It wraps the angular $http service
    // ------------------------------------------------------------------------
    export class ApiServiceBase {
        req: IRequestMethod;

        constructor($http: ng.IHttpService, private prefix: string) {

            this.req = (method: string, url: string, data?: any) => {
                var httpPromise = $http({
                    method: method,
                    url: url,
                    data: data
                });

                var ret: IBusinessPromise<any> = <IBusinessPromise<any>>{};

                ret.success = (callback) => {
                    httpPromise.success((dataReceived) => {
                        convertDateStringsToDates(dataReceived);
                        httpPromise.success(callback);
                    });
                    return ret;
                };

                ret.error = (business, unexpected) => {
                    httpPromise.error((datareceived, status, headers) => {
                        if (status == 500 && angular.isDefined(datareceived.isBusiness) && datareceived.isBusiness) {
                            business(datareceived, status, headers);
                        } else {
                            unexpected(datareceived, status, headers);
                        }
                    });
                    return ret;
                };
                return ret;
            };
        }

        // --- Creates an URL according to the specified parameters
        url(...params: any[]) {
            return this.prefix + "/" + params.join("/");
        }
    }

    // ========================================================================
    // API for internationalization
    // ========================================================================

}

Core.initApis();