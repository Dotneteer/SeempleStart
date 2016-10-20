var Account;
(function (Account) {
    /*
     * This class defines a DTO that describes an external user login
     */
    var UserLoginInfoDto = (function () {
        function UserLoginInfoDto(loginProvider, providerKey) {
            this.loginProvider = loginProvider;
            this.providerKey = providerKey;
        }
        return UserLoginInfoDto;
    }());
    Account.UserLoginInfoDto = UserLoginInfoDto;
    /*
     * This class defines a DTO for the ManageAccounts view
     */
    var ManageAccountDto = (function () {
        function ManageAccountDto() {
        }
        return ManageAccountDto;
    }());
    Account.ManageAccountDto = ManageAccountDto;
})(Account || (Account = {}));
//# sourceMappingURL=account.ng.dto.js.map