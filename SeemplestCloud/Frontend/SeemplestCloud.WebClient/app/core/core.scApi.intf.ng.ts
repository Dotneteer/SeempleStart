module Core {
    /*
     * This class describes a business error
     */
    export interface IBusinessError {
        type: string;
        message: string;
        code: number;
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


} 