using System;

namespace Seemplest.Core.DataAccess.Attributes
{
    /// <summary>
    /// This attribute marks that a data record is immutable, so it cannot be modified
    /// after being read from the database.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ImmutableRecordAttribute : ImoAttributeBase
    {
    }
}