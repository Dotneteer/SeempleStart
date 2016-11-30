var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var YwApi;
(function (YwApi) {
    function initYwApi() {
        Main.appModule
            .service('ywapi', ['$http', YwApiService]);
    }
    YwApi.initYwApi = initYwApi;
    // --- This regex matches with a date of yyyy-MM-ddTHH:mm:ss format
    var dateTimeStringRegex = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})/;
    // --- Converts a JSON string into a date time value
    function convertDateStringsToDates(input) {
        // Ha a paraméter nem object típusú, visszatérünk.
        if (!input || typeof input !== "object")
            return;
        for (var key in input) {
            if (!input.hasOwnProperty(key))
                continue;
            var value = input[key];
            // Ellenőrzés, hogy az adattag JSON dátum-e
            if (typeof value === "string" && (value.match(dateTimeStringRegex))) {
                input[key] = moment(value).toDate();
            }
            else if (typeof value === "object") {
                // Ha objektum, akkor rekurzívan ellenőrzünk
                convertDateStringsToDates(value);
            }
        }
    }
    /*
     * This class is intended to be the base of all Api implementations
     */
    var ApiBase = (function () {
        function ApiBase(prefix) {
            this.prefix = prefix;
        }
        ApiBase.prototype.url = function () {
            var params = [];
            for (var _i = 0; _i < arguments.length; _i++) {
                params[_i - 0] = arguments[_i];
            }
            return this.prefix + "/" + params.join("/");
        };
        return ApiBase;
    }());
    YwApi.ApiBase = ApiBase;
    /*
     * This class implements the entire Younderwater API
     */
    var YwApiService = (function () {
        function YwApiService($http) {
            // --- Wrap the angular service
            var req = function (method, url, data) {
                var httpPromise = $http({
                    method: method,
                    url: url,
                    data: data
                });
                var ret = {};
                ret.success = function (callback) {
                    httpPromise.success(function (dataReceived) {
                        convertDateStringsToDates(dataReceived);
                        httpPromise.success(callback);
                    });
                    return ret;
                };
                ret.expectedError = function (callback) {
                    httpPromise.error(callback);
                    return ret;
                };
                ret.unexpectedError = function (callback) {
                    httpPromise.error(callback);
                    return ret;
                };
                return ret;
            };
            this.UserManagement = new UserManagementApi(req, 'manageuser');
            this.DiveLog = new DiveLogApi(req, 'diveLog');
        }
        return YwApiService;
    }());
    YwApi.YwApiService = YwApiService;
    /*
     * Implementation of the user management API
     */
    var UserManagementApi = (function (_super) {
        __extends(UserManagementApi, _super);
        function UserManagementApi(req, prefix) {
            var _this = _super.call(this, prefix) || this;
            _this.req = req;
            return _this;
        }
        UserManagementApi.prototype.getAccountInfo = function () {
            return this.req('GET', this.url(''));
        };
        return UserManagementApi;
    }(ApiBase));
    YwApi.UserManagementApi = UserManagementApi;
    var DiveLogApi = (function (_super) {
        __extends(DiveLogApi, _super);
        function DiveLogApi(req, prefix) {
            var _this = _super.call(this, prefix) || this;
            _this.req = req;
            return _this;
        }
        DiveLogApi.prototype.getAllDivesOfUser = function () {
            return this.req('GET', this.url(''));
        };
        DiveLogApi.prototype.getDiveById = function (diveId) {
            return this.req('GET', this.url(diveId));
        };
        DiveLogApi.prototype.registerDiveLogEntry = function (entry) {
            return this.req('POST', this.url(''), entry);
        };
        DiveLogApi.prototype.modifyDiveLogEntry = function (entry) {
            return this.req('PUT', this.url(''), entry);
        };
        DiveLogApi.prototype.removeDiveLogEntry = function (diveId) {
            return this.req('DELETE', this.url(diveId));
        };
        return DiveLogApi;
    }(ApiBase));
    YwApi.DiveLogApi = DiveLogApi;
})(YwApi || (YwApi = {}));
YwApi.initYwApi();
//# sourceMappingURL=ywapi.service.js.map