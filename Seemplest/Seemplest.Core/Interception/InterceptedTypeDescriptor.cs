using System;
using System.Collections.Generic;
using System.Reflection;

namespace Seemplest.Core.Interception
{
    /// <summary>
    /// This class provides information about an intercepted type
    /// </summary>
    internal class InterceptedTypeDescriptor
    {
        /// <summary>
        /// The type that has been intercepted
        /// </summary>
        public Type InterceptedType { get; set; }

        /// <summary>
        /// Stores the method information of the intercepted type
        /// </summary>
        public List<MethodInfo> Methods { get; set; }

        /// <summary>
        /// Describes the targer expressions for the wrapper type
        /// </summary>
        public Func<IMethodCallDescriptor, IMethodResultDescriptor>[] TargetMethods { get; set; }
        
        /// <summary>
        /// Gets the wrapper type
        /// </summary>
        public Type WrapperType { get; set; }
    }
}