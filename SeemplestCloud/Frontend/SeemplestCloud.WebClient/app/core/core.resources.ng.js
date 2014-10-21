var Core;
(function (Core) {
    /*
    * Initializes the components of the main view
    */
    function initResources() {
    }
    Core.initResources = initResources;

    // --- Describes a resource string
    var ResourceStringDto = (function () {
        function ResourceStringDto(code, value) {
            this.code = code;
            this.value = value;
        }
        return ResourceStringDto;
    })();
    Core.ResourceStringDto = ResourceStringDto;

    
})(Core || (Core = {}));

Core.initResources();
//# sourceMappingURL=core.resources.ng.js.map
