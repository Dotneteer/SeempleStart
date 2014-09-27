module Core {
   /*
    * Initializes the components of the main view
    */
    export function initCommonFilters() {
        Main.appModule
            .filter('zeroAsEmpty', [zeroAsEmpty])
            .filter('dateAsString', [dateAsStringFilter])
            .filter('longText', [longTextFilter]);
    }
    
    /*
     * This filter shows the zero value as an empty string
     */
    export function zeroAsEmpty() {
        return (value: number) => {
            if (angular.isNumber(value)) {
                return value == 0 ? '' : value.toString();
            } else {
                return value.toString();
            }
        }
    }

    /*
     * This filter shows a text with a specified maximum length
     */
    export function longTextFilter() {
        var postfix = "...";

        return (input: string, maxLength: number) => {
            var ret;

            if (input.length > (maxLength + postfix.length)) {
                ret = input.substr(0, maxLength) + postfix;
            } else {
                ret = input;
            }
            return ret;
        };
    }

    /*
     * This filter converts a date to a string
     */
    export function dateAsStringFilter() {
        return (date: Date) => {
            return moment(date).toDate();
        }
    }
}

Core.initCommonFilters();