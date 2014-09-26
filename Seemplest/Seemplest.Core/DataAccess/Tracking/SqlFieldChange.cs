namespace Seemplest.Core.DataAccess.Tracking
{
    /// <summary>
    /// This class represent change information for a field
    /// </summary>
    public class SqlFieldChange
    {
        /// <summary>
        /// Gets the previous field value
        /// </summary>
        public readonly object PreviousValue;

        /// <summary>
        /// Gets the new field value
        /// </summary>
        public readonly object NewValue;

        /// <summary>
        /// Initializes this instance with the specified previous and new field values
        /// </summary>
        /// <param name="previousValue">Previous field value</param>
        /// <param name="newValue">New field value</param>
        public SqlFieldChange(object previousValue, object newValue)
        {
            PreviousValue = previousValue;
            NewValue = newValue;
        }
    }
}