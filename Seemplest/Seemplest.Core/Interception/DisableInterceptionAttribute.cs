using System;

namespace Seemplest.Core.Interception
{
    /// <summary>
    /// This attribute can be put on types marking that type should not be intercepted.
    /// When such a type is instantiated from a service container, it won't be
    /// intercepted even if any base type in the inheritance chain is makred to be
    /// intercepted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class DisableInterceptionAttribute: Attribute
    {
    }
}