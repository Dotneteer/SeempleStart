using System;

namespace Seemplest.Core.TypeResolution
{
    /// <summary>
    /// This interface describes the behavior of an object that is able to resolve
    /// a string name to a type.
    /// </summary>
    public interface ITypeResolver
    {
        /// <summary>
        /// Resolves the specified name to a <see cref="Type"/> instance.
        /// </summary>
        /// <param name="name">Name to resolve</param>
        /// <returns><see cref="Type"/> instance</returns>
        Type Resolve(string name);
    }
}