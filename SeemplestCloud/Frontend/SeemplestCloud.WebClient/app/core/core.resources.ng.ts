module Core {
    /*
     * Initializes the components of the main view
     */
    export function initResources() {
    }

    // --- Describes a resource string
    export class ResourceStringDto {
        code: string;
        value: string;

        constructor(code: string, value: string) {
            this.code = code;
            this.value = value;
        }
    }

    // --- Defines the resource manager service
    export interface IResourceManagerService {
        reset: () => void;
        getCurrentUiCulture: () => string;
        getResourceValue: (category: string, code: string) => string;
    }
}

Core.initResources();
 