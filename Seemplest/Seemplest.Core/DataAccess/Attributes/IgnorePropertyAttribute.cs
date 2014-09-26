using System;

namespace Seemplest.Core.DataAccess.Attributes
{
    /// <summary>
    /// This attribute marks a property as one to be ignored when mapping it to or 
    /// from a database table.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnorePropertyAttribute : ImoAttributeBase
    {
    }
}