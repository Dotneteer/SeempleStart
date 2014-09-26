using System.ComponentModel;
using System.Data.SqlClient;
using System.Xml.Linq;
using Seemplest.Core.Configuration.ResourceConnections;

namespace Seemplest.MsSql.Configuration
{
    /// <summary>
    /// This connection provider instantiates an <see cref="SqlConnection"/> object
    /// from a connection string.
    /// </summary>
    [DisplayName("SqlConnection")]
    public class SqlConnectionProvider : SingleValueResourceConnectionProvider<string>
    {
        /// <summary>
        /// Creates a new instance with the specified name and Value.
        /// </summary>
        /// <param name="name">Setting name</param>
        /// <param name="value">Setting value</param>
        public SqlConnectionProvider(string name, string value)
            : base(name, value)
        {
        }

        /// <summary>
        /// Creates a new instance from the specified XML element
        /// </summary>
        /// <param name="element"></param>
        public SqlConnectionProvider(XElement element)
            : base(element)
        {
        }

        /// <summary>
        /// Creates a new resource connection object from the settings.
        /// </summary>
        /// <returns>Newly created resource object</returns>
        public override object GetResourceConnectionFromSettings()
        {
            return new SqlConnection(Value);
        }
    }
}