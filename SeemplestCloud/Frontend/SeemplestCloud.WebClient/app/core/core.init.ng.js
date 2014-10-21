var Core;
(function (Core) {
    // ------------------------------------------------------------------------
    // This variable holds the name of this module
    // ------------------------------------------------------------------------
    Core.appModuleName = 'Core';

    // ------------------------------------------------------------------------
    // This variable represents this module for the external world
    // ------------------------------------------------------------------------
    Core.appModule;

    // ------------------------------------------------------------------------
    // Initializes this module. This method should be
    // called before initializing any other module components
    // ------------------------------------------------------------------------
    function initModule() {
        Core.appModule = angular.module(Core.appModuleName, []);
    }
    Core.initModule = initModule;
})(Core || (Core = {}));

Core.initModule();
//# sourceMappingURL=core.init.ng.js.map
