module Core {
    /*
     * Defines the behavior of a controller that provides simple search functionality
     */
    export interface ISimpleSearch {
        searchKey: string;
        clearSearchKey: () => void;
    } 
} 