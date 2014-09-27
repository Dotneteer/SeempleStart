module Account {

    /*
     * This class defines a DTO that describes an external user login
     */
    export class UserLoginInfoDto {
        loginProvider: string;
        providerKey: string;

        constructor(loginProvider?: string, providerKey?: string) {
            this.loginProvider = loginProvider;
            this.providerKey = providerKey;
        }
    }

    /*
     * This class defines a DTO for the ManageAccounts view
     */
    export class ManageAccountDto {
        hasPassword: boolean;
        logins: UserLoginInfoDto[];
        phoneNumber: string;
        twoFactor: boolean;
        browserRemembered: boolean;
    }
} 