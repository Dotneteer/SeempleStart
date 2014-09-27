module Core {
    /*
     * Initializes the components of the main view
     */
    export function initCommonDirectives() {
        Main.appModule
            .directive('noClick', [() => new NoClickDirective()])
            .directive('pagination', [() => new PaginationDirective()])
            .directive('required', [() => new RequiredDirective()])
            .directive('matches', [() => new MatchesWidthDirective()])
            .directive('maxlen', [() => new LengthRangeDirective()])
            .directive('minlen', [() => new LengthRangeDirective()]);
    }

    /*
     * Represents the scope of the core controller
     */
    export interface ICoreScope extends ng.IScope {
        showError?: (ngModelController: ng.INgModelController, error: string) => any;
    }

    /*
     * Represents the core controller
     */
    export class CoreController {
        showError(ngModelController: ng.INgModelController, error: string) {
            return !!ngModelController.$dirty && ngModelController.$error[error];
        }

        constructor($scope: ICoreScope) {
            $scope.showError = this.showError;
        }
    }

    /*
     * This class is intended to be the base of other directives
     */
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

        /*
         * Obtains the controller from a parameter array
         */
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

    /*
     * Apply this directive to an element to disable the click event
     */
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

    /*
     * Apply this component for pagination
     */
    export class PaginationDirective extends BaseDirective {
        currentPage; number;
        totalPages: number;
        currentlyShownPages: number[];

        static createButton(label: string, clickEvent: (eventObject: JQueryEventObject) => void): ng.IAugmentedJQuery {
            var button = angular.element('<li><a href=#>' + label + '</a></li>');
            button.click({ page: label }, clickEvent);
            return button;
        }
        constructor() {
            super();
            var that = this;
            that.currentPage = 0;
            this.restrict = 'A';
            that.currentlyShownPages = [];

            this.link = (scope: ng.IScope, instanceElement: any, instanceAttributes: ng.IAttributes) => {

                instanceElement.addClass('pagination');
                var searchFunc: string;
                if (instanceAttributes.$attr['gotoPageFunction']) {
                    searchFunc = instanceElement.attr(instanceAttributes.$attr['gotoPageFunction']);
                } else {
                    searchFunc = "gotoPage";
                }


                var handleButtonClick = function (eventObject: JQueryEventObject) {
                    eventObject.preventDefault();
                    var jQueryTarget = angular.element(eventObject.delegateTarget);
                    if (!jQueryTarget.hasClass("disabled")) {
                        var page = eventObject.data['page'];
                        var pageNumber: number;
                        if (page === '>') {
                            pageNumber = this.currentPage + 1;
                            if (pageNumber > this.totalPages) {
                                pageNumber = this.totalPages;
                            }
                        }
                        else if (page === '>>') {
                            pageNumber = this.totalPages;
                        }
                        else if (page === '<<') {
                            pageNumber = 1;
                        }
                        else if (page === '<') {
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

                var hasPageButton = (): boolean => {
                    var returnValue: boolean = false;
                    angular.forEach(instanceElement.children(), function (item: any) {
                        var jqueryObject = angular.element(item);
                        if (jqueryObject.text() === this.currentPage.toString()) {
                            returnValue = true;
                        }
                    });
                    return returnValue;
                };

                var refresh = function (goToFirstPage: boolean) {
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
                        var resetPage: boolean = goToFirstPage || (this.currentPage > this.totalPages);
                        if (resetPage) {
                            this.currentPage = 1;
                        }
                        var needToReset: boolean = (!hasPageButton()) || goToFirstPage;
                        if (needToReset) {
                            instanceElement.empty();
                            var maxButtons: number = 5;
                            var firstPageNumber: number = this.currentPage;
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
                        angular.forEach(instanceElement.children(), (item: any) => {
                            var jqueryObject = angular.element(item);
                            var text: string = jqueryObject.text();
                            if (thisCurrentPage === thisTotalPages && (text === ">" || text === ">>")) {
                                jqueryObject.addClass('disabled');
                            }
                            else if (thisCurrentPage === 1 && (text === "<" || text === "<<")) {
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
                scope.$on('pageLoadCompleted', () => { refresh(false); });
                scope.$on('searchCompleted', () => { refresh(true); });
            };
        }
    }

    /*
     * Directive to check an input value for the 'required' attribute
     */
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

    /*
     * Directive to check the maximum length of a string
     */
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

    /*
     * Directive to check the maximum length of a string
     */
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

    /*
     * Directive to check the length range of a string
    */
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

    /*
     * Directive to check the maximum length of a string
     */
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
}

Core.initCommonDirectives();