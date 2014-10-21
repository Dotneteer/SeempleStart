var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var Core;
(function (Core) {
    // ------------------------------------------------------------------------
    // Initializes the angular objects defined in this module fragment
    // ------------------------------------------------------------------------
    function initApis() {
        Core.appModule.service('intlnApi', ['$http', 'currentSpot', InternationalizationApi]).service('intlnSrv', ['intlnApi', InternationalizationService]);
    }
    Core.initApis = initApis;

    // ------------------------------------------------------------------------
    // This regex matches with a date of yyyy-MM-ddTHH:mm:ss format
    // ------------------------------------------------------------------------
    var dateTimeStringRegex = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})/;

    // ------------------------------------------------------------------------
    // Converts a JSON string into a date time value
    // ------------------------------------------------------------------------
    function convertDateStringsToDates(input) {
        if (!input || typeof input !== "object")
            return;

        for (var key in input) {
            if (!input.hasOwnProperty(key))
                continue;

            var value = input[key];

            // --- Check whether the data is a JSON datetime
            if (typeof value === "string" && (value.match(dateTimeStringRegex))) {
                input[key] = moment(value).toDate();
            } else if (typeof value === "object") {
                // --- Go on recursively with the check
                convertDateStringsToDates(value);
            }
        }
    }

    

    

    // ------------------------------------------------------------------------
    // This structure represents a WebApi call response
    // ------------------------------------------------------------------------
    var ResponseData = (function () {
        function ResponseData() {
        }
        return ResponseData;
    })();
    Core.ResponseData = ResponseData;

    

    

    

    

    // ------------------------------------------------------------------------
    // This class is intended to be the base class of all API service objects.
    // It wraps the angular $http service
    // ------------------------------------------------------------------------
    var ApiServiceBase = (function () {
        function ApiServiceBase($http, currentSpot, prefix) {
            var _this = this;
            this.currentSpot = currentSpot;
            this.prefix = prefix;
            // --- This method is used as the default callback for "reject"
            this.defaultReject = function (response) {
                if (response.handled)
                    return;
                if (response.hasError) {
                    if (response.status == 500 && angular.isDefined(response.data.isBusiness) && response.data.isBusiness) {
                        _this.currentSpot.setError(response.data.reasonCode, response.data.message);
                    } else {
                        _this.currentSpot.setError("INFRA", response.data.message);
                    }
                }
            };
            this.request = function (method, url, data) {
                var httpPromise = $http({
                    method: method,
                    url: url,
                    data: data
                });

                // --- Callback information
                var businessPromise = {};
                var successCallback;
                var errorCallbacks = [];
                var unexpectedCallback;
                var acceptedCallback;
                var concludedCallback;
                var rejectedCallback = _this.defaultReject;

                // --- Last response information
                var lastResponse;

                // --- Set the "onSuccess" callback
                businessPromise.onSuccess = function (callback) {
                    successCallback = callback;
                    return businessPromise;
                };

                // --- Set the "onError" callback
                businessPromise.onError = function (reasonCode, callback) {
                    errorCallbacks.push({ reasonCode: reasonCode, callback: callback });
                    return businessPromise;
                };

                // --- Set the "unexpected" callback
                businessPromise.unexpected = function (callback) {
                    unexpectedCallback = callback;
                    return businessPromise;
                };

                // --- Set the "accept" callback
                businessPromise.accept = function (callback) {
                    acceptedCallback = callback;
                    return businessPromise;
                };

                // --- Set the "reject" callback
                businessPromise.reject = function (callback) {
                    rejectedCallback = callback;
                    return businessPromise;
                };

                // --- Set the "conclude" callback
                businessPromise.conclude = function (callback) {
                    concludedCallback = callback;
                    return businessPromise;
                };

                // --- Call the WebApi and handle issues
                businessPromise.go = function () {
                    currentSpot.resetError();
                    httpPromise.success(function (dataReceived, status, headers) {
                        lastResponse = {
                            data: dataReceived, status: status, headers: headers, hasError: false, handled: false
                        };
                        if (angular.isDefined(successCallback)) {
                            successCallback(lastResponse);
                        }
                    }).error(function (dataReceived, status, headers) {
                        lastResponse = {
                            data: dataReceived, status: status, headers: headers, hasError: true, handled: false
                        };
                        if (status == 500 && angular.isDefined(dataReceived.isBusiness) && dataReceived.isBusiness) {
                            for (var i = 0; i < errorCallbacks.length; i++) {
                                if (errorCallbacks[i].reasonCode == dataReceived.reasonCode && angular.isDefined(errorCallbacks[i].callback)) {
                                    errorCallbacks[i].callback(lastResponse);
                                    lastResponse.handled = true;
                                    return;
                                }
                            }
                        }
                        if (angular.isDefined(unexpectedCallback)) {
                            unexpectedCallback(lastResponse);
                        }
                    }).then(function () {
                        try  {
                            if (angular.isDefined(acceptedCallback)) {
                                acceptedCallback(lastResponse);
                            }
                        } catch (ex) {
                        }
                        if (angular.isDefined(concludedCallback)) {
                            concludedCallback(lastResponse);
                        }
                    }, function () {
                        try  {
                            if (angular.isDefined(rejectedCallback)) {
                                rejectedCallback(lastResponse);
                            }
                        } catch (ex) {
                        }
                        if (angular.isDefined(concludedCallback)) {
                            concludedCallback(lastResponse);
                        }
                    });
                };

                businessPromise.go();
                return businessPromise;
            };
        }
        // --- Creates an URL according to the specified parameters
        ApiServiceBase.prototype.url = function () {
            var params = [];
            for (var _i = 0; _i < (arguments.length - 0); _i++) {
                params[_i] = arguments[_i + 0];
            }
            return this.prefix + "/" + params.join("/");
        };
        return ApiServiceBase;
    })();
    Core.ApiServiceBase = ApiServiceBase;

    // ========================================================================
    // API for internationalization
    // ========================================================================
    // ------------------------------------------------------------------------
    // DTO for string resources
    // ------------------------------------------------------------------------
    var ResourceStringDto = (function () {
        function ResourceStringDto() {
        }
        return ResourceStringDto;
    })();
    Core.ResourceStringDto = ResourceStringDto;

    

    // ------------------------------------------------------------------------
    // Operations of the internationalization API
    // ------------------------------------------------------------------------
    var InternationalizationApi = (function (_super) {
        __extends(InternationalizationApi, _super);
        function InternationalizationApi($http, currentSpot) {
            _super.call(this, $http, currentSpot, '../api/intln');
        }
        InternationalizationApi.prototype.getCurrentCulture = function () {
            return this.request('GET', this.url('current'));
        };

        InternationalizationApi.prototype.getServiceMessages = function () {
            return this.request('GET', this.url('servicemessages'));
        };
        return InternationalizationApi;
    })(Core.ApiServiceBase);
    Core.InternationalizationApi = InternationalizationApi;

    

    // ------------------------------------------------------------------------
    // Service implementation
    // ------------------------------------------------------------------------
    var InternationalizationService = (function () {
        function InternationalizationService(intlnApi) {
            var _this = this;
            this.currentCulture = "";
            this.serviceMessages = [];
            this.getCurrentCulture = function () {
                return _this.currentCulture;
            };
            this.getServiceMessage = function (code, pars) {
                var message = "<none>";
                for (var i = 0; i < _this.serviceMessages.length; i++) {
                    var resource = _this.serviceMessages[i];
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
            };
            this.api = intlnApi;
            this.api.getCurrentCulture().onSuccess(function (response) {
                _this.currentCulture = response.data;
            }).go();
            this.api.getServiceMessages().onSuccess(function (response) {
                _this.serviceMessages = response.data;
            }).go();
        }
        return InternationalizationService;
    })();
    Core.InternationalizationService = InternationalizationService;
})(Core || (Core = {}));

Core.initApis();
//# sourceMappingURL=core.api.ng.js.map
