module Core {

    // ------------------------------------------------------------------------
    // Initializes the angular objects defined in this module fragment
    // ------------------------------------------------------------------------
    export function initCommonDirectives() {
        Core.appModule
            .filter('zeroAsEmpty', [zeroAsEmpty])
            .filter('dateAsString', [dateAsStringFilter])
            .filter('longText', [longTextFilter])
            .directive('noClick', [() => new NoClickDirective()])
            .directive('required', [() => new RequiredDirective()])
            .directive('matches', [() => new MatchesWidthDirective()])
            .directive('maxlen', [() => new LengthRangeDirective()])
            .directive('minlen', [() => new LengthRangeDirective()])
            .directive('strongpsw', [() => new StrongPasswordDirective()]);
    }

    // ------------------------------------------------------------------------
    // Defines the behavior of a controller that provides simple search functionality
    // ------------------------------------------------------------------------
    export interface ISimpleSearch {
        searchKey: string;
        clearSearchKey: () => void;
    } 

    // ------------------------------------------------------------------------
    // Represents the scope of the core controller
    // ------------------------------------------------------------------------
    export interface ICoreScope extends ng.IScope {
        showError?: (ngModelController: ng.INgModelController, error: string) => any;
    }

    // ------------------------------------------------------------------------
    // Represents the core controller
    // ------------------------------------------------------------------------
    export class CoreController {

        constructor($scope: ICoreScope) {
            $scope.showError = this.showError;
        }

        showError(ngModelController: ng.INgModelController, error: string) {
            return !!ngModelController.$dirty && ngModelController.$error[error];
        }
    }

    // ------------------------------------------------------------------------
    // This filter shows zeros as empty strings
    // ------------------------------------------------------------------------
    export function zeroAsEmpty() {
        return (value: number) => {
            if (angular.isNumber(value)) {
                return value == 0 ? '' : value.toString();
            } else {
                return value.toString();
            }
        }
    }

    // ------------------------------------------------------------------------
    // This filter shows a text with a specified maximum length
    // ------------------------------------------------------------------------
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

    // ------------------------------------------------------------------------
    // This filter converts a date to a string
    // ------------------------------------------------------------------------
    export function dateAsStringFilter() {
        return (date: Date) => {
            return moment(date).toDate();
        }
    }

    // ------------------------------------------------------------------------
    // This class is intended to be the base of other directives
    // ------------------------------------------------------------------------
    export class BaseDirective implements ng.IDirective {
        public priority: number;
        public template: string;
        public templateUrl: string;
        public replace: boolean;
        public transclude: any;
        public restrict: string;
        public scope: any;
        public link: (scope: ng.IScope, instanceElement: any, instanceAttributes: ng.IAttributes, controller: any ) => void;
        public compile: (templateElement: any, templateAttributes: ng.IAttributes, transclude: (scope: ng.IScope, cloneLinkingFn: Function) => void) => any;
        public controller: (...injectables: any[]) => void;
        public isEmpty: (value: any) => boolean;
        public require: string[];
        constructor() {
            this.isEmpty = value =>
                angular.isUndefined(value) || value === '' || value === null || value !== value;
        }

        // --- Obtains the controller from a parameter array
        public static getControllerFromParameterArray: (controller: any) => ng.INgModelController =
            (controller: any) => {
              var currentController: ng.INgModelController;
                if (angular.isArray(controller) && controller.length > 0) {
                    currentController = controller[0];
                } else {
                    currentController = controller;
                }
                return currentController;
        };
    }

    // ------------------------------------------------------------------------
    // Apply this directive to an element to disable the click event
    // ------------------------------------------------------------------------
    export class NoClickDirective extends BaseDirective {

        constructor() {
            super();
            this.restrict = 'A';
            this.link = (scope: ng.IScope, element: ng.IAugmentedJQuery) => {
                element.click((eventObject: JQueryEventObject) => {
                    eventObject.preventDefault();
                });
            };
        }
    }

    // ------------------------------------------------------------------------
    // Directive to check an input value for the 'required' attribute
    // ------------------------------------------------------------------------
    export class RequiredDirective extends BaseDirective {
        constructor() {
            super();
            this.restrict = 'A';
            this.require = ['?ngModel'];
            this.link = (scope: ICoreScope, element: ng.IAugmentedJQuery, attributes: ng.IAttributes, controller: any) => {
                var currentController: ng.INgModelController = BaseDirective.getControllerFromParameterArray(controller);
                if (!currentController) return;
                var validator = value => {
                    if ((this.isEmpty(value) || value === false)) {
                        currentController.$setValidity('required', false);
                        return value;
                    } else {
                        currentController.$setValidity('required', true);
                        return value;
                    }
                };

                currentController.$formatters.push(validator);
                currentController.$parsers.unshift(validator);
            };
        }
    }

    // ------------------------------------------------------------------------
    // Directive to check the maximum length of a string
    // ------------------------------------------------------------------------
    export class MaxLengthDirective extends BaseDirective {
        constructor() {
            super();
            this.restrict = 'A';
            this.require = ['?ngModel'];
            this.link = (scope: ICoreScope, element: ng.IAugmentedJQuery, attributes: ng.IAttributes, controller: any) => {
                var maxLength: number = parseInt(element.attr(attributes.$attr['maxlen']));
                var currentController: ng.INgModelController = BaseDirective.getControllerFromParameterArray(controller);
                if (!currentController) return;
                var validator = value => {
                    if (!this.isEmpty(value) && value.length > maxLength) {
                        currentController.$setValidity('maxlen', false);
                        return value;
                    } else {
                        currentController.$setValidity('maxlen', true);
                        return value;
                    }
                };

                currentController.$formatters.push(validator);
                currentController.$parsers.unshift(validator);
            };
        }
    }

    // ------------------------------------------------------------------------
    // Directive to check the minimum length of a string
    // ------------------------------------------------------------------------
    export class MinLengthDirective extends BaseDirective {
        constructor() {
            super();
            this.restrict = 'A';
            this.require = ['?ngModel'];
            this.link = (scope: ICoreScope, element: ng.IAugmentedJQuery, attributes: ng.IAttributes, controller: any) => {
                var minLength: number = parseInt(element.attr(attributes.$attr['minlen']));
                var currentController: ng.INgModelController = BaseDirective.getControllerFromParameterArray(controller);
                if (!currentController) return;
                var validator = value => {
                    if (!this.isEmpty(value) && value.length < minLength) {
                        currentController.$setValidity('minlen', false);
                        return value;
                    } else {
                        currentController.$setValidity('minlen', true);
                        return value;
                    }
                };

                currentController.$formatters.push(validator);
                currentController.$parsers.unshift(validator);
            };
        }
    }

    // ------------------------------------------------------------------------
    // Directive to check the length range of a string
    // ------------------------------------------------------------------------
    export class LengthRangeDirective extends BaseDirective {
        constructor() {
            super();
            this.restrict = 'A';
            this.require = ['?ngModel'];
            this.link = (scope: ICoreScope, element: ng.IAugmentedJQuery, attributes: ng.IAttributes, controller: any) => {
                var minLength: number = parseInt(element.attr(attributes.$attr['minlen']));
                var maxLength: number = parseInt(element.attr(attributes.$attr['maxlen']));
                var currentController: ng.INgModelController = BaseDirective.getControllerFromParameterArray(controller);
                if (!currentController) return;
                var validator = value => {
                    if (!this.isEmpty(value) && angular.isDefined(minLength) && value.length < minLength) {
                        currentController.$setValidity('lenrange', false);
                        return value;
                    } else if (!this.isEmpty(value) && angular.isDefined(maxLength) && value.length > maxLength) {
                        currentController.$setValidity('lenrange', false);
                            return value;
                    } else
                    {
                        currentController.$setValidity('lenrange', true);
                        return value;
                    }
                };

                currentController.$formatters.push(validator);
                currentController.$parsers.unshift(validator);
            };
        }
    }

    // ------------------------------------------------------------------------
    // Directive to check whether the contents of a field matches with another
    // ------------------------------------------------------------------------
    export class MatchesWidthDirective extends BaseDirective {
        constructor() {
            super();
            this.restrict = 'A';
            this.require = ['?ngModel'];
            this.link = (scope: any, element: ng.IAugmentedJQuery, attributes: any, controller: any) => {
                var currentController: ng.INgModelController = BaseDirective.getControllerFromParameterArray(controller);
                if (!currentController) return;
                var validator = value => {
                    var comparisonValue = attributes.matches;
                    if (!value || !comparisonValue) {
                        currentController.$setValidity('matches', true);
                        return value;
                    }
                    if (value === comparisonValue) {
                        currentController.$setValidity('matches', true);
                        return value;
                    } else {
                        currentController.$setValidity('matches', false);
                        return value;
                    }
                };

                currentController.$formatters.push(validator);
                currentController.$parsers.unshift(validator);
                attributes.$observe('matches', comparisonModel => validator(controller.$viewValue));
            };
        }
    }

    // ------------------------------------------------------------------------
    // Directive to check whether the specified value can be a string password
    // ------------------------------------------------------------------------
    export class StrongPasswordDirective extends BaseDirective {
        constructor() {
            super();
            this.restrict = 'A';
            this.require = ['?ngModel'];
            this.link = (scope: any, element: ng.IAugmentedJQuery, attributes: any, controller: any) => {
                var currentController: ng.INgModelController = BaseDirective.getControllerFromParameterArray(controller);
                if (!currentController) return;
                var validator = value => {
                    if (!value || !angular.isString(value)) {
                        currentController.$setValidity('strongpsw', false);
                        return value;
                    }
                    var hasDigit = false;
                    var hasNonLetterOrDigit = false;
                    var hasLowerCase = false;
                    var hasUpperCase = false;
                    for (var i = 0; i < value.length; i++) {
                        var c = value.charAt(i);
                        hasDigit = hasDigit || (c >= '0' && c <= '9');
                        hasNonLetterOrDigit = hasNonLetterOrDigit ||
                            !((c >= '0' && c <= '9' || c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z'));
                        hasLowerCase = hasLowerCase || c >= 'a' && c <= 'z';
                        hasUpperCase = hasUpperCase || c >= 'A' && c <= 'Z';
                    }
                    currentController.$setValidity('strongpsw', hasDigit && hasNonLetterOrDigit && hasLowerCase && hasUpperCase);
                    return value;
                }

                currentController.$formatters.push(validator);
                currentController.$parsers.unshift(validator);
            };
        }
    }

    // ------------------------------------------------------------------------
    // Defines the abstract scope of a generic popup controller
    // ------------------------------------------------------------------------
    export interface IPopupCtrlScopeBase<TWm> extends ng.IScope {
        // --- The viewmodel object of the popup
        model: TWm;

        // --- This method is called when the user executes OK.
        ok: () => void;

        // --- Override this method to define what to do with OK. Return true
        // --- for signing successful operation; otherwise, false
        onOk: () => boolean;

        // --- This flag signs that the OK button should be disabled.
        disableOk: boolean;

        // --- This method is called when the user executes Cancel.
        cancel: () => void;

        // --- Override this method to define what to do with Cancel. Return true
        // --- for signing successful operation; otherwise, false
        onCancel: () => boolean;
    }

    // ------------------------------------------------------------------------
    // This controller manages the user invitation entry popup
    // ------------------------------------------------------------------------
    export class PopupCtrlBase<TWm> {
        public static $inject = ['$scope', '$modalInstance'];

        constructor(
            $scope: IPopupCtrlScopeBase<TWm>,
            $modalInstance: ng.ui.bootstrap.IModalServiceInstance) {

            $scope.disableOk = false;
            $scope.onOk = () => { return true; }
            $scope.onCancel = () => { return true; }

            $scope.ok = () => {
                $scope.disableOk = true;
                if ($scope.onOk()) {
                    $modalInstance.close();
                }
            };

            $scope.cancel = () => {
                if ($scope.onCancel()) {
                    $modalInstance.dismiss('cancel');
                }
            };
        }
    }
}

Core.initCommonDirectives();