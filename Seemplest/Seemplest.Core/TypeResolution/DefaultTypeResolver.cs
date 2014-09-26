using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Seemplest.Core.TypeResolution
{
    /// <summary>
    /// This class uses predefined assemblies and namespaces to look up type names.
    /// </summary>
    public class DefaultTypeResolver: ChainedTypeResolverBase
    {
        private readonly TypeResolverConfigurationSettings _settings;

        /// <summary>
        /// Creates a new instance of this class using the specified default resolver.
        /// </summary>
        public DefaultTypeResolver()
            : base(null)
        {
        }

        /// <summary>
        /// Creates a new instance of this class using the specified parent resolver.
        /// </summary>
        /// <param name="parentResolver">Parent type resolver</param>
        public DefaultTypeResolver(ITypeResolver parentResolver) 
            : base(parentResolver)
        {
        }

        /// <summary>
        /// Creates a new instance of this class using the specified default resolver and
        /// type resolution settings.
        /// </summary>
        /// <param name="settings">Type resolution settings</param>
        /// <param name="parentResolver">Default type resolver</param>
        public DefaultTypeResolver(TypeResolverConfigurationSettings settings, ITypeResolver parentResolver = null) 
            : base(parentResolver)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            _settings = settings;
        }

        /// <summary>
        /// Resolves the specified name to a <see cref="Type"/> instance.
        /// </summary>
        /// <param name="name">Name to resolve</param>
        /// <returns><see cref="Type"/> instance</returns>
        public override Type ResolveLocally(string name)
        {
            var result = ResolveIntrinsicTypes(name);
            return result ?? ResolveWithSearch(name);
        }

        /// <summary>
        /// Tries to resolve types from intrinsic type names
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static Type ResolveIntrinsicTypes(string name)
        {
            switch (name)
            {
                case "bool": return typeof(bool);
                case "char": return typeof(char);
                case "byte": return typeof(byte);
                case "sbyte": return typeof(sbyte);
                case "short": return typeof(short);
                case "ushort": return typeof(ushort);
                case "int": return typeof(int);
                case "uint": return typeof(uint);
                case "long": return typeof(long);
                case "ulong": return typeof(ulong);
                case "decimal": return typeof(decimal);
                case "float": return typeof(float);
                case "double": return typeof(double);
                case "string": return typeof(string);
                case "object": return typeof(object);
                case "Type": return typeof(Type);
                case "Guid": return typeof (Guid);
                case "DateTime": return typeof(DateTime);
                default: return null;
            }
        }

        /// <summary>
        /// Resolves a type by searching the specified assemblies and namespaces
        /// </summary>
        /// <param name="name">name to resolve</param>
        /// <returns></returns>
        private Type ResolveWithSearch(string name)
        {
            // --- No settings defined, no resolution can be done.
            if (_settings == null) return null;

            // --- Collect matching types
            var typesFound = new HashSet<Type>();
            foreach (var asm in _settings.AssemblyNames)
            {
                foreach (var ns in _settings.Namespaces)
                {
                    try
                    {
                        var typeName = String.Format("{2}.{0}.{1}, {2}", ns, name, asm);
                        var type = Type.GetType(typeName);
                        if (type != null)
                        {
                            typesFound.Add(type);
                        }
                        typeName = String.Format("{0}.{1}, {2}", ns, name, asm);
                        type = Type.GetType(typeName);
                        if (type != null)
                        {
                            typesFound.Add(type);
                        }
                    }
                    catch (SystemException)
                    {
                        // --- This exception is intentionally caught to avoid exceptions
                        // --- coming from unloadable assemblies
                    }
                }
            }
            if (typesFound.Count == 0) return null;
            if (typesFound.Count == 1) return typesFound.First();
            throw new ConfigurationErrorsException(String.Format(
                "Type '{0}' has {1} candidates, it cannot be unambigously resolved.", 
                name, typesFound.Count));
        }
    }
}