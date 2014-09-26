using System;

namespace Seemplest.Core.DataAccess.Attributes
{
    /// <summary>
    /// This attribute marks a property as one representing a calculated column value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CalculatedAttribute : ImoAttributeBase
    {
    }
}