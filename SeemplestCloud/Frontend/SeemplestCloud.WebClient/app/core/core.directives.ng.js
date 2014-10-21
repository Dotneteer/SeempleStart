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
    function initCommonDirectives() {
        Core.appModule.filter('zeroAsEmpty', [zeroAsEmpty]).filter('dateAsString', [dateAsStringFilter]).filter('longText', [longTextFilter]).directive('noClick', [function () {
                return new NoClickDirective();
            }]).directive('required', [function () {
                return new RequiredDirective();
            }]).directive('matches', [function () {
                return new MatchesWidthDirective();
            }]).directive('maxlen', [function () {
                return new LengthRangeDirective();
            }]).directive('minlen', [function () {
                return new LengthRangeDirective();
            }]).directive('strongpsw', [function () {
                return new StrongPasswordDirective();
            }]);
    }
    Core.initCommonDirectives = initCommonDirectives;

    

    

    // ------------------------------------------------------------------------
    // Represents the core controller
    // ------------------------------------------------------------------------
    var CoreController = (function () {
        function CoreController($scope) {
            $scope.showError = this.showError;
        }
        CoreController.prototype.showError = function (ngModelController, error) {
            return !!ngModelController.$dirty && ngModelController.$error[error];
        };
        return CoreController;
    })();
    Core.CoreController = CoreController;

    // ------------------------------------------------------------------------
    // This filter shows zeros as empty strings
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

    // ------------------------------------------------------------------------
    // This class is intended to be the base of other directives
    // ------------------------------------------------------------------------
    var BaseDirective = (function () {
        function BaseDirective() {
            this.isEmpty = function (value) {
                return angular.isUndefined(value) || value === '' || value === null || value !== value;
            };
        }
        BaseDirective.getControllerFromParameterArray = function (controller) {
            var currentController;
            if (angular.isArray(controller) && controller.length > 0) {
                currentController = controller[0];
            } else {
                currentController = controller;
            }
            return currentController;
        };
        return BaseDirective;
    })();
    Core.BaseDirective = BaseDirective;

    // ------------------------------------------------------------------------
    // Apply this directive to an element to disable the click event
    // ------------------------------------------------------------------------
    var NoClickDirective = (function (_super) {
        __extends(NoClickDirective, _super);
        function NoClickDirective() {
            _super.call(this);
            this.restrict = 'A';
            this.link = function (scope, element) {
                element.click(function (eventObject) {
                    eventObject.preventDefault();
                });
            };
        }
        return NoClickDirective;
    })(BaseDirective);
    Core.NoClickDirective = NoClickDirective;

    // ------------------------------------------------------------------------
    // Directive to check an input value for the 'required' attribute
    // ------------------------------------------------------------------------
    var RequiredDirective = (function (_super) {
        __extends(RequiredDirective, _super);
        function RequiredDirective() {
            var _this = this;
            _super.call(this);
            this.restrict = 'A';
            this.require = ['?ngModel'];
            this.link = function (scope, element, attributes, controller) {
                var currentController = BaseDirective.getControllerFromParameterArray(controller);
                if (!currentController)
                    return;
                var validator = function (value) {
                    if ((_this.isEmpty(value) || value === false)) {
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
        return RequiredDirective;
    })(BaseDirective);
    Core.RequiredDirective = RequiredDirective;

    // ------------------------------------------------------------------------
    // Directive to check the maximum length of a string
    // ------------------------------------------------------------------------
    var MaxLengthDirective = (function (_super) {
        __extends(MaxLengthDirective, _super);
        function MaxLengthDirective() {
            var _this = this;
            _super.call(this);
            this.restrict = 'A';
            this.require = ['?ngModel'];
            this.link = function (scope, element, attributes, controller) {
                var maxLength = parseInt(element.attr(attributes.$attr['maxlen']));
                var currentController = BaseDirective.getControllerFromParameterArray(controller);
                if (!currentController)
                    return;
                var validator = function (value) {
                    if (!_this.isEmpty(value) && value.length > maxLength) {
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
        return MaxLengthDirective;
    })(BaseDirective);
    Core.MaxLengthDirective = MaxLengthDirective;

    // ------------------------------------------------------------------------
    // Directive to check the minimum length of a string
    // ------------------------------------------------------------------------
    var MinLengthDirective = (function (_super) {
        __extends(MinLengthDirective, _super);
        function MinLengthDirective() {
            var _this = this;
            _super.call(this);
            this.restrict = 'A';
            this.require = ['?ngModel'];
            this.link = function (scope, element, attributes, controller) {
                var minLength = parseInt(element.attr(attributes.$attr['minlen']));
                var currentController = BaseDirective.getControllerFromParameterArray(controller);
                if (!currentController)
                    return;
                var validator = function (value) {
                    if (!_this.isEmpty(value) && value.length < minLength) {
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
        return MinLengthDirective;
    })(BaseDirective);
    Core.MinLengthDirective = MinLengthDirective;

    // ------------------------------------------------------------------------
    // Directive to check the length range of a string
    // ------------------------------------------------------------------------
    var LengthRangeDirective = (function (_super) {
        __extends(LengthRangeDirective, _super);
        function LengthRangeDirective() {
            var _this = this;
            _super.call(this);
            this.restrict = 'A';
            this.require = ['?ngModel'];
            this.link = function (scope, element, attributes, controller) {
                var minLength = parseInt(element.attr(attributes.$attr['minlen']));
                var maxLength = parseInt(element.attr(attributes.$attr['maxlen']));
                var currentController = BaseDirective.getControllerFromParameterArray(controller);
                if (!currentController)
                    return;
                var validator = function (value) {
                    if (!_this.isEmpty(value) && angular.isDefined(minLength) && value.length < minLength) {
                        currentController.$setValidity('lenrange', false);
                        return value;
                    } else if (!_this.isEmpty(value) && angular.isDefined(maxLength) && value.length > maxLength) {
                        currentController.$setValidity('lenrange', false);
                        return value;
                    } else {
                        currentController.$setValidity('lenrange', true);
                        return value;
                    }
                };

                currentController.$formatters.push(validator);
                currentController.$parsers.unshift(validator);
            };
        }
        return LengthRangeDirective;
    })(BaseDirective);
    Core.LengthRangeDirective = LengthRangeDirective;

    // ------------------------------------------------------------------------
    // Directive to check whether the contents of a field matches with another
    // ------------------------------------------------------------------------
    var MatchesWidthDirective = (function (_super) {
        __extends(MatchesWidthDirective, _super);
        function MatchesWidthDirective() {
            _super.call(this);
            this.restrict = 'A';
            this.require = ['?ngModel'];
            this.link = function (scope, element, attributes, controller) {
                var currentController = BaseDirective.getControllerFromParameterArray(controller);
                if (!currentController)
                    return;
                var validator = function (value) {
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
                attributes.$observe('matches', function (comparisonModel) {
                    return validator(controller.$viewValue);
                });
            };
        }
        return MatchesWidthDirective;
    })(BaseDirective);
    Core.MatchesWidthDirective = MatchesWidthDirective;

    // ------------------------------------------------------------------------
    // Directive to check whether the specified value can be a string password
    // ------------------------------------------------------------------------
    var StrongPasswordDirective = (function (_super) {
        __extends(StrongPasswordDirective, _super);
        function StrongPasswordDirective() {
            _super.call(this);
            this.restrict = 'A';
            this.require = ['?ngModel'];
            this.link = function (scope, element, attributes, controller) {
                var currentController = BaseDirective.getControllerFromParameterArray(controller);
                if (!currentController)
                    return;
                var validator = function (value) {
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
                        hasNonLetterOrDigit = hasNonLetterOrDigit || !((c >= '0' && c <= '9' || c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z'));
                        hasLowerCase = hasLowerCase || c >= 'a' && c <= 'z';
                        hasUpperCase = hasUpperCase || c >= 'A' && c <= 'Z';
                    }
                    currentController.$setValidity('strongpsw', hasDigit && hasNonLetterOrDigit && hasLowerCase && hasUpperCase);
                    return value;
                };

                currentController.$formatters.push(validator);
                currentController.$parsers.unshift(validator);
            };
        }
        return StrongPasswordDirective;
    })(BaseDirective);
    Core.StrongPasswordDirective = StrongPasswordDirective;

    

    // ------------------------------------------------------------------------
    // This controller manages the user invitation entry popup
    // ------------------------------------------------------------------------
    var PopupCtrlBase = (function () {
        function PopupCtrlBase($scope, $modalInstance) {
            $scope.disableOk = false;
            $scope.onOk = function () {
                return true;
            };
            $scope.onCancel = function () {
                return true;
            };

            $scope.ok = function () {
                $scope.disableOk = true;
                if ($scope.onOk()) {
                    $modalInstance.close();
                }
            };

            $scope.cancel = function () {
                if ($scope.onCancel()) {
                    $modalInstance.dismiss('cancel');
                }
            };
        }
        PopupCtrlBase.$inject = ['$scope', '$modalInstance'];
        return PopupCtrlBase;
    })();
    Core.PopupCtrlBase = PopupCtrlBase;
})(Core || (Core = {}));

Core.initCommonDirectives();
//# sourceMappingURL=core.directives.ng.js.map
