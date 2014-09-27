module YwApi {

    /*
     * This class describes a business error
     */
    export interface IBusinessError {
        type: string;
        message: string;
        code: number;
    }

    /*
     * This interface describes a validation error
     */
    export interface IValidationError {
        propertyName: string;
        errorKey: string;
    }

    /*
     * This interface defines a business promise callback of type T
     */
    export interface IBusinessPromiseCallback<T> {
        (data: T, status: number, headers: (headerName: string) => string): void;
    }

    /*
     * This class defines a business promise that returns a type of TReturn with an expected
     * type of error (TEcpectedError)
     */
    export interface IBusinessPromise<TReturn, TExpectedError> {
        success(callback: IBusinessPromiseCallback<TReturn>): IBusinessPromise<TReturn, TExpectedError>;
        expectedError(callback: IBusinessPromiseCallback<TExpectedError>): IBusinessPromise<TReturn, TExpectedError>;
        unexpectedError(callback: IBusinessPromiseCallback<IBusinessError>): IBusinessPromise<TReturn, TExpectedError>;
    }

    /*
     * This interface describes a request method
     */
    export interface IRequestMethod {
        (method: string, url: string, data?: any): IBusinessPromise<any, any>;
    }

    /*
     * This interface defines the full Younderwater API
     */
    export interface IApiService {
        UserManagement: IUserManagementApi;
        DiveLog: IDiveLogApi;
    }

    /*
     * This interface defines the operations of the user management API
     */
    export interface IUserManagementApi {
        getAccountInfo(): IBusinessPromise<Account.ManageAccountDto, any>;
    }

    /*
     * This interface defines the operations of the dive log management API
     */
    export interface IDiveLogApi {
        getAllDivesOfUser(): IBusinessPromise<Dive.DiveLogEntryDto[], any>;
        getDiveById(diveId: number): IBusinessPromise<Dive.DiveLogEntryDto, any>;
        registerDiveLogEntry(entry: Dive.DiveLogEntryDto): IBusinessPromise<number, any>;
        modifyDiveLogEntry(entry: Dive.DiveLogEntryDto): IBusinessPromise<any, any>;
        removeDiveLogEntry(diveId: number): IBusinessPromise<any, any>;
    }
} 