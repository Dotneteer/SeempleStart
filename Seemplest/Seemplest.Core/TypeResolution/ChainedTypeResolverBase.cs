using System;

namespace Seemplest.Core.TypeResolution
{
    /// <summary>
    /// This class implements a chained type resolver that turns to a default type resolver
    /// if it is unable to resolve a type by its name.
    /// </summary>
    public abstract class ChainedTypeResolverBase: ITypeResolver
    {
        private readonly ITypeResolver _parentResolver;

        /// <summary>
        /// Creates a type resolver using the specified type resolver instance as
        /// a fallback.
        /// </summary>
        /// <param name="parentResolver">Fallback type resolver</param>
        protected ChainedTypeResolverBase(ITypeResolver parentResolver = null) 
        {
            _parentResolver = parentResolver ?? new TrivialTypeResolver();
        }

        /// <summary>
        /// Resolves the specified name to a <see cref="Type"/> instance.
        /// </summary>
        /// <param name="name">Name to resolve</param>
        /// <returns><see cref="Type"/>Type instance if resolutions is OK; otherwise, false</returns>
        public Type Resolve(string name)
        {
            var result = ResolveLocally(name);
            return result ?? _parentResolver.Resolve(name);
        }

        /// <summary>
        /// Resolves the specified name to a <see cref="Type"/> instance.
        /// </summary>
        /// <param name="name">Name to resolve</param>
        /// <returns><see cref="Type"/>This implementation always return null</returns>
        public abstract Type ResolveLocally(string name);
    }
}
