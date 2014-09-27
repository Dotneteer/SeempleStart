module YwApi {

    export function initYwApi() {
        Main.appModule
            .service('ywapi', ['$http', YwApiService]);
    }

    // --- This regex matches with a date of yyyy-MM-ddTHH:mm:ss format
    var dateTimeStringRegex = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})/;

    // --- Converts a JSON string into a date time value
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

    /*
     * This class is intended to be the base of all Api implementations
     */
    export class ApiBase {
        url(...params: any[]) {
            return this.prefix + "/" + params.join("/");
        }

        constructor(private prefix: string) {
        }
    }

    /*
     * This class implements the entire Younderwater API
     */
    export class YwApiService implements IApiService {
        UserManagement: IUserManagementApi;
        DiveLog: IDiveLogApi;

        constructor($http: ng.IHttpService) {
            // --- Wrap the angular service
            var req = (method: string, url: string, data?: any) => {
                var httpPromise = $http({
                    method: method,
                    url: url,
                    data: data
                });
                var ret: IBusinessPromise<any, any> = <IBusinessPromise<any, any>>{};
                ret.success = (callback) => {
                    httpPromise.success((dataReceived) => {
                        convertDateStringsToDates(dataReceived);
                        httpPromise.success(callback);
                    });
                    return ret;
                };
                ret.expectedError = (callback) => {
                    httpPromise.error(callback);
                    return ret;
                };
                ret.unexpectedError = (callback) => {
                    httpPromise.error(callback);
                    return ret;
                };

                return ret;
            };

            this.UserManagement = new UserManagementApi(req, 'manageuser');
            this.DiveLog = new DiveLogApi(req, 'diveLog');
        }
    }

    /*
     * Implementation of the user management API
     */
    export class UserManagementApi extends ApiBase implements IUserManagementApi {
        constructor(private req: IRequestMethod, prefix: string) {
            super(prefix);
        }

        getAccountInfo() {
            return this.req('GET', this.url(''));
        }
    }

    export class DiveLogApi extends ApiBase implements IDiveLogApi {
        constructor(private req: IRequestMethod, prefix: string) {
            super(prefix);
        }

        getAllDivesOfUser() {
            return this.req('GET', this.url(''));
        }
        
        getDiveById(diveId: number) {
            return this.req('GET', this.url(diveId));
        }

        registerDiveLogEntry(entry: Dive.DiveLogEntryDto) {
            return this.req('POST', this.url(''), entry);
        }

        modifyDiveLogEntry(entry: Dive.DiveLogEntryDto) {
            return this.req('PUT', this.url(''), entry);
        }
        removeDiveLogEntry(diveId: number) {
            return this.req('DELETE', this.url(diveId));
        }
    }
}

YwApi.initYwApi();