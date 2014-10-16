module Core {

    // ------------------------------------------------------------------------
    // Initializes the angular objects defined in this module fragment
    // ------------------------------------------------------------------------
    export function initApis() {
        Core.appModule
            .service('intlnApi', ['$http', InternationalizationApi])
            .service('intlnSrv', ['intlnApi', InternationalizationService]);
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
    // his class defines a business promise that returns a type of TReturn
    // ------------------------------------------------------------------------
    export interface IBusinessPromise<TReturn> {
        onSuccess: (callback: IBusinessPromiseCallback<TReturn>) => IBusinessPromise<TReturn>;
        onError: (reasonCode: string, business: IBusinessPromiseCallback<IBusinessError>) => IBusinessPromise<TReturn>;
        unexpected: (callback: IBusinessPromiseCallback<IInfrastructureError>) => IBusinessPromise<TReturn>;
        accept: (callback: () => void) => IBusinessPromise<TReturn>;
        reject: (callback: () => void) => IBusinessPromise<TReturn>;
        conclude: (callback: () => void) => IBusinessPromise<TReturn>;
        go: () => void;
    }

    // ------------------------------------------------------------------------
    // This interface describes a request method
    // ------------------------------------------------------------------------
    export interface IRequestMethod {
        (method: string, url: string, data?: any): IBusinessPromise<any>;
    }

    // ------------------------------------------------------------------------
    // This structure represents an error callback
    // ------------------------------------------------------------------------
    interface IErrorCallback {
        reasonCode: string;
        callback: IBusinessPromiseCallback<IBusinessError>;
    }

    // ------------------------------------------------------------------------
    // This class is intended to be the base class of all API service objects.
    // It wraps the angular $http service
    // ------------------------------------------------------------------------
    export class ApiServiceBase {
        request: IRequestMethod;

        constructor($http: ng.IHttpService, private prefix: string) {

            this.request = (method: string, url: string, data?: any) => {
                var httpPromise = $http({
                    method: method,
                    url: url,
                    data: data
                });

                var businessPromise: IBusinessPromise<any> = <IBusinessPromise<any>>{};
                var successCallback: IBusinessPromiseCallback<any>;
                var errorCallbacks: IErrorCallback[] = [];
                var unexpectedCallback: IBusinessPromiseCallback<any>;
                var acceptedCallback: () => void;
                var rejectedCallback: () => void;
                var concludedCallback: () => void;

                // --- Set the "onSuccess" callback
                businessPromise.onSuccess = (callback) => {
                    successCallback = callback;
                    return businessPromise;
                };

                // --- Set the "onError" callback
                businessPromise.onError = (reasonCode, callback) => {
                    errorCallbacks.push({ reasonCode: reasonCode, callback: callback });
                    return businessPromise;
                };

                // --- Set the "unexpected" callback
                businessPromise.unexpected = (callback) => {
                    unexpectedCallback = callback;
                    return businessPromise;
                };

                // --- Set the "accept" callback
                businessPromise.accept = (callback) => {
                    acceptedCallback = callback;
                    return businessPromise;
                };

                // --- Set the "reject" callback
                businessPromise.reject = (callback) => {
                    rejectedCallback = callback;
                    return businessPromise;
                };

                // --- Set the "conclude" callback
                businessPromise.conclude = (callback) => {
                    concludedCallback = callback;
                    return businessPromise;
                };

                // --- Call the WebApi and handle issues
                businessPromise.go = () => {
                    httpPromise.success((dataReceived, status, headers) => {
                        if (angular.isDefined(successCallback)) {
                            successCallback(dataReceived, status, headers);
                        }
                    }).error((dataReceived, status, headers) => {
                        if (status == 500 && angular.isDefined(dataReceived.isBusiness) && dataReceived.isBusiness) {
                            for (var i = 0; i < errorCallbacks.length; i++) {
                                if (errorCallbacks[i].reasonCode == dataReceived.reasonCode
                                    && angular.isDefined(errorCallbacks[i].callback)) {
                                    errorCallbacks[i].callback(dataReceived, status, headers);
                                    return;
                                }
                            }
                        }
                        if (angular.isDefined(unexpectedCallback)) {
                            unexpectedCallback(dataReceived, status, headers);
                        }
                    }).then(() => {
                        try {
                            if (angular.isDefined(acceptedCallback)) {
                                acceptedCallback();
                            }
                        } catch (ex) {
                        }
                        if (angular.isDefined(concludedCallback)) {
                            concludedCallback();
                        }
                    }, () => {
                        try {
                            if (angular.isDefined(rejectedCallback)) {
                                rejectedCallback();
                            }
                        } catch (ex) {
                        }
                        if (angular.isDefined(concludedCallback)) {
                            concludedCallback();
                        }
                    });
                }

                return businessPromise;
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
    // ------------------------------------------------------------------------
    // DTO for string resources
    // ------------------------------------------------------------------------
    export class ResourceStringDto {
        code: string;
        value: string;
    }

    // ------------------------------------------------------------------------
    // Operations of the internationalization API
    // ------------------------------------------------------------------------
    export interface IInternationalizationApi {
        getCurrentCulture: () => Core.IBusinessPromise<string>;
        getServiceMessages: () => Core.IBusinessPromise<ResourceStringDto[]>;
    }

    // ------------------------------------------------------------------------
    // Operations of the internationalization API
    // ------------------------------------------------------------------------
    export class InternationalizationApi extends Core.ApiServiceBase implements IInternationalizationApi {

        constructor($http: ng.IHttpService) {
            super($http, '../api/intln');
        }

        getCurrentCulture() {
            return this.request('GET', this.url('current'));
        }

        getServiceMessages() {
            return this.request('GET', this.url('servicemessages'));
        }
    }

    // ========================================================================
    // Service for internationalization
    // ========================================================================
    // ------------------------------------------------------------------------
    // Service interface
    // ------------------------------------------------------------------------
    export interface IInternationalizationService {
        getCurrentCulture: () => string;
        getServiceMessage: (code: string, pars?: any) => string;
    }

    // ------------------------------------------------------------------------
    // Service implementation
    // ------------------------------------------------------------------------
    export class InternationalizationService implements IInternationalizationService {
        api: IInternationalizationApi;
        currentCulture: string = "";
        serviceMessages: ResourceStringDto[] = [];

        constructor(intlnApi: IInternationalizationApi) {
            this.api = intlnApi;
            this.api.getCurrentCulture()
                .onSuccess((dataReceived) => {
                    this.currentCulture = dataReceived;
            }).go();
            this.api.getServiceMessages()
                .onSuccess((dataReceived) => {
                    this.serviceMessages = dataReceived;
                }).go();
        }

        getCurrentCulture = () => {
            return this.currentCulture;
        }

        getServiceMessage = (code: string, pars?: any) => {
            var message = "<none>";
            for (var i = 0; i < this.serviceMessages.length; i++) {
                var resource = this.serviceMessages[i];
                if (resource.code == code) {
                    message = resource.value;
                    if (angular.isDefined(pars)) {
                        for (var prop in pars) {
                            if (message.hasOwnProperty(prop)) {
                                message = message.replace('{{' + prop + '}}', pars[prop]);
                            }
                        }
                    }
                    break;
                }
            }
            return message;
        }
    }
}

Core.initApis();