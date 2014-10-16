var Core;
(function (Core) {
    // ------------------------------------------------------------------------
    // Initializes the angular objects defined in this module fragment
    // ------------------------------------------------------------------------
    function initCommonFilters() {
        Core.appModule.filter('zeroAsEmpty', [zeroAsEmpty]).filter('dateAsString', [dateAsStringFilter]).filter('longText', [longTextFilter]);
    }
    Core.initCommonFilters = initCommonFilters;

    // ------------------------------------------------------------------------
    // This filter shows the zero value as an empty string
    // ------------------------------------------------------------------------
    function zeroAsEmpty() {
        return function (value) {
            if (angular.isNumber(value)) {
                return value == 0 ? '' : value.toString();
            } else {
                return value.toString();
            }
        };
    }
    Core.zeroAsEmpty = zeroAsEmpty;

    // ------------------------------------------------------------------------
    // This filter shows a text with a specified maximum length
    // ------------------------------------------------------------------------
    function longTextFilter() {
        var postfix = "...";

        return function (input, maxLength) {
            var ret;

            if (input.length > (maxLength + postfix.length)) {
                ret = input.substr(0, maxLength) + postfix;
            } else {
                ret = input;
            }
            return ret;
        };
    }
    Core.longTextFilter = longTextFilter;

    // ------------------------------------------------------------------------
    // This filter converts a date to a string
    // ------------------------------------------------------------------------
    function dateAsStringFilter() {
        return function (date) {
            return moment(date).toDate();
        };
    }
    Core.dateAsStringFilter = dateAsStringFilter;
})(Core || (Core = {}));

Core.initCommonFilters();
//# sourceMappingURL=core.filters.ng.js.map
