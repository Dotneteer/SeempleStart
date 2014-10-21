var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var Core;
(function (Core) {
    /*
    * Initializes the components of the main view
    */
    function initCommonDirectives() {
        Main.appModule.directive('noClick', [function () {
                return new NoClickDirective();
            }]).directive('pagination', [function () {
                return new PaginationDirective();
            }]).directive('required', [function () {
                return new RequiredDirective();
            }]).directive('matches', [function () {
                return new MatchesWidthDirective();
            }]).directive('maxlen', [function () {
                return new LengthRangeDirective();
            }]).directive('minlen', [function () {
                return new LengthRangeDirective();
            }]);
    }
    Core.initCommonDirectives = initCommonDirectives;

    

    /*
    * Represents the core controller
    */
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

    /*
    * This class is intended to be the base of other directives
    */
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

    /*
    * Apply this directive to an element to disable the click event
    */
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

    /*
    * Apply this component for pagination
    */
    var PaginationDirective = (function (_super) {
        __extends(PaginationDirective, _super);
        function PaginationDirective() {
            _super.call(this);
            var that = this;
            that.currentPage = 0;
            this.restrict = 'A';
            that.currentlyShownPages = [];

            this.link = function (scope, instanceElement, instanceAttributes) {
                instanceElement.addClass('pagination');
                var searchFunc;
                if (instanceAttributes.$attr['gotoPageFunction']) {
                    searchFunc = instanceElement.attr(instanceAttributes.$attr['gotoPageFunction']);
                } else {
                    searchFunc = "gotoPage";
                }

                var handleButtonClick = function (eventObject) {
                    eventObject.preventDefault();
                    var jQueryTarget = angular.element(eventObject.delegateTarget);
                    if (!jQueryTarget.hasClass("disabled")) {
                        var page = eventObject.data['page'];
                        var pageNumber;
                        if (page === '>') {
                            pageNumber = this.currentPage + 1;
                            if (pageNumber > this.totalPages) {
                                pageNumber = this.totalPages;
                            }
                        } else if (page === '>>') {
                            pageNumber = this.totalPages;
                        } else if (page === '<<') {
                            pageNumber = 1;
                        } else if (page === '<') {
                            pageNumber = this.currentPage - 1;
                            if (pageNumber < 1) {
                                pageNumber = 1;
                            }
                        } else {
                            pageNumber = parseInt(page);
                        }
                        this.currentPage = pageNumber;
                        scope.$apply(searchFunc + '(' + pageNumber.toString() + ')');
                    }
                };

                var hasPageButton = function () {
                    var returnValue = false;
                    angular.forEach(instanceElement.children(), function (item) {
                        var jqueryObject = angular.element(item);
                        if (jqueryObject.text() === this.currentPage.toString()) {
                            returnValue = true;
                        }
                    });
                    return returnValue;
                };

                var refresh = function (goToFirstPage) {
                    if (instanceAttributes.$attr['pageCount']) {
                        this.totalPages = scope.$eval(instanceElement.attr(instanceAttributes.$attr["pageCount"]));
                    } else {
                        this.totalPages = scope.$eval("pageCount");
                    }

                    if (instanceAttributes.$attr['currentPage']) {
                        this.currentPage = scope.$eval(instanceElement.attr(instanceAttributes.$attr["currentPage"]));
                    } else {
                        this.currentPage = scope.$eval("currentPage");
                    }

                    if (this.totalPages > 0) {
                        var resetPage = goToFirstPage || (this.currentPage > this.totalPages);
                        if (resetPage) {
                            this.currentPage = 1;
                        }
                        var needToReset = (!hasPageButton()) || goToFirstPage;
                        if (needToReset) {
                            instanceElement.empty();
                            var maxButtons = 5;
                            var firstPageNumber = this.currentPage;
                            while ((firstPageNumber + 4) > this.totalPages) {
                                firstPageNumber--;
                            }
                            if (firstPageNumber < 1) {
                                firstPageNumber = 1;
                            }
                            this.currentlyShownPages = [];
                            for (var i = firstPageNumber; i <= this.totalPages; i++) {
                                if (i < firstPageNumber + maxButtons) {
                                    this.currentlyShownPages.push(i);
                                } else {
                                    break;
                                }
                            }

                            instanceElement.append(PaginationDirective.createButton('<<', handleButtonClick));
                            instanceElement.append(PaginationDirective.createButton('<', handleButtonClick));
                            for (var j = 0; j < this.currentlyShownPages.length; j++) {
                                var button = PaginationDirective.createButton(this.currentlyShownPages[j].toString(), handleButtonClick);
                                instanceElement.append(button);
                            }
                            instanceElement.append(PaginationDirective.createButton('>', handleButtonClick));
                            instanceElement.append(PaginationDirective.createButton('>>', handleButtonClick));
                        }
                        var thisCurrentPage = this.currentPage;
                        var thisTotalPages = this.totalPages;
                        angular.forEach(instanceElement.children(), function (item) {
                            var jqueryObject = angular.element(item);
                            var text = jqueryObject.text();
                            if (thisCurrentPage === thisTotalPages && (text === ">" || text === ">>")) {
                                jqueryObject.addClass('disabled');
                            } else if (thisCurrentPage === 1 && (text === "<" || text === "<<")) {
                                jqueryObject.addClass('disabled');
                            } else {
                                jqueryObject.removeClass('disabled');
                            }
                            if (text === thisCurrentPage.toString()) {
                                jqueryObject.addClass('active');
                            } else {
                                jqueryObject.removeClass('active');
                            }
                        });
                    } else {
                        instanceElement.empty();
                    }
                };
                scope.$on('pageLoadCompleted', function () {
                    refresh(false);
                });
                scope.$on('searchCompleted', function () {
                    refresh(true);
                });
            };
        }
        PaginationDirective.createButton = function (label, clickEvent) {
            var button = angular.element('<li><a href=#>' + label + '</a></li>');
            button.click({ page: label }, clickEvent);
            return button;
        };
        return PaginationDirective;
    })(BaseDirective);
    Core.PaginationDirective = PaginationDirective;

    /*
    * Directive to check an input value for the 'required' attribute
    */
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

    /*
    * Directive to check the maximum length of a string
    */
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

    /*
    * Directive to check the maximum length of a string
    */
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

    /*
    * Directive to check the length range of a string
    */
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

    /*
    * Directive to check the maximum length of a string
    */
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
})(Core || (Core = {}));

Core.initCommonDirectives();
//# sourceMappingURL=commonDirectives.ng.js.map
