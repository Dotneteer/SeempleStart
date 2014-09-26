using System;

namespace Seemplest.Core.TypeResolution
{
    /// <summary>
    /// This class represents the default type resolver
    /// </summary>
    public class TrivialTypeResolver: ITypeResolver 
    {
        /// <summary>
        /// Resolves the specified name to a <see cref="Type"/> instance.
        /// </summary>
        /// <param name="name">Name to resolve</param>
        /// <returns><see cref="Type"/> instance</returns>
        public Type Resolve(string name)
        {
            return Type.GetType(name);
        }
    }
}