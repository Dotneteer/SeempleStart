using System;

namespace Seemplest.Core.Interception
{
    /// <summary>
    /// This attribute can be put on types marking that type as intercepted.
    /// When such a type is instantiated from a service container, it will be
    /// automatically intercepted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class InterceptedAttribute: Attribute
    {
    }
}