using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text;

namespace Seemplest.Core.Exceptions
{
    /// <summary>
    /// This exception is raised when a service registry contains duplicated 
    /// container names.
    /// </summary>
    public class DuplicatedContainerNameException: Exception
    {
        private readonly List<string> _duplicatedNames;

        /// <summary>
        /// Creates a new instance with the specified service type.
        /// </summary>
        /// <param name="duplicatedNames">Duplicated container names</param>
        public DuplicatedContainerNameException(IEnumerable<string> duplicatedNames)
        {
            _duplicatedNames = new List<string>(duplicatedNames);
        }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        /// <returns>
        /// The error message that explains the reason for the exception, or an empty string("").
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override string Message
        {
            get
            {
                var nameList = new StringBuilder();
                foreach (var name in _duplicatedNames)
                {
                    if (nameList.Length > 0) nameList.Append(", ");
                    nameList.Append(name);
                }
                return string.Format("The following container name{0} duplicated: {1}",
                    _duplicatedNames.Count > 1 ? "s are" : " is", nameList);
            }
        }
        /// <summary>
        /// Initializes a new instance of the Exception class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The SerializationInfo that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The StreamingContext that contains contextual information about the source or destination.
        /// </param>
        [ExcludeFromCodeCoverage]
        protected DuplicatedContainerNameException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}