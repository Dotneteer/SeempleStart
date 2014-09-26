namespace Seemplest.Core.Configuration
{
    #pragma warning disable 1591
    public class ConfigurationConstants
    {
        // --- Name of the core log
        public const string CORE_LOG_NAME = "Seemples Core Log";

        // --- Name of the core source
        public const string AZURE_COMPONENTS_SOURCE = "Seemplest Components";

        // --- Azure core component categories
        public const short CORE_CATEGORY = 1000;

        // --- Event IDs
        public const int ASPECT_INFRASTRUCTURE_ID = 1;
        public const int TASK_PROCESSOR_STARTED_ID = 2;
        public const int TASK_PROCESSOR_STOPPED_ID = 3;
        public const int TASK_PROCESSOR_STOPPED_WITH_TIMEOUT_ID = 4;
        public const int POISONING_MESSAGE_FOUND_ID = 5;
        public const int TASK_EXECUTION_INTERRUPTED_ID = 6;
        public const int TASK_EXECUTION_FAILED_ID = 7;
        public const int TASK_PROCESSOR_HOST_EXCEPTION_ID = 8;
        public const int TASK_PROCESSOR_HOST_WARNING_ID = 9;
        public const int AUDIT_LOG_MANAGER_WAS_CONFIGURED_ID = 10;
        public const int AUDIT_LOG_FAILED_ID = 11;
        public const int DIAGNOSTICS_LOG_FAILED_ID = 12;
    }
    #pragma warning restore 1591
}