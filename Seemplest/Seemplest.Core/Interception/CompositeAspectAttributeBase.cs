namespace Seemplest.Core.Interception
{
    /// <summary>
    /// This class is intended to be a composite attribute that can be decorated with other 
    /// <see cref="AspectAttributeBase"/> instances.
    /// </summary>
    /// <remarks>
    /// When scanning for the aspect attributes, any CompositeAspectAttribute works as if
    /// the composite attribute class were replaced with the attributes decorating the
    /// composite attribute class.
    /// </remarks>
    public abstract class CompositeAspectAttributeBase : AspectAttributeBase
    {
    }
}