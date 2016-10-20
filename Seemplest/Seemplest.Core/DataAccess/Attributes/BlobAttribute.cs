using System;

namespace Seemplest.Core.DataAccess.Attributes
{
    /// <summary>
    /// This attribute marks a property as one representing blob value
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class BlobAttribute : ImoAttributeBase
    {
    }
}