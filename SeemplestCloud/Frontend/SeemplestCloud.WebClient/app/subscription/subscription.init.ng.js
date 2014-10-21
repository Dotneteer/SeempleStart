var Subscription;
(function (Subscription) {
    // ------------------------------------------------------------------------
    // This variable holds the name of this module
    // ------------------------------------------------------------------------
    Subscription.appModuleName = 'Subscription';

    // ------------------------------------------------------------------------
    // This variable represents this module for the external world
    // ------------------------------------------------------------------------
    Subscription.appModule;

    // ------------------------------------------------------------------------
    // Initializes this module. This method should be
    // called before initializing any other module components
    // ------------------------------------------------------------------------
    function initModule() {
        Subscription.appModule = angular.module(Subscription.appModuleName, []);
    }
    Subscription.initModule = initModule;
})(Subscription || (Subscription = {}));

Subscription.initModule();
//# sourceMappingURL=subscription.init.ng.js.map
