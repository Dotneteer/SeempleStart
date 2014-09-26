using System;

namespace Seemplest.Core.DataAccess.Attributes
{
    /// <summary>
    /// This attribute marks a property as one representing version (timestamp) column.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class VersionColumnAttribute : ImoAttributeBase
    {
    }
}