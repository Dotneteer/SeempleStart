using System;
using System.Globalization;
using System.Reflection;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace SeemplestBlocks.Testing.WinRtExtensions
{
    /// <summary>
    /// Contains methods to help with creating assertions.
    /// </summary>
    public static class AssertHelper
    {
        /// <summary>
        /// Creates a custom <see cref="AssertFailedException"/> and provides information for the exception.
        /// </summary>
        /// <param name="assertionName">The name of the assertion (typically the name of the assertion method).</param>
        /// <param name="message">A message for the assertion.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <exception cref="ArgumentException"><i>assertionName</i> is null</exception>
        /// <exception cref="AssertFailedException"><i>assertionName</i> is an empty string</exception>
        public static void AssertFailed(string assertionName, string message, params object[] parameters)
        {
            if (String.IsNullOrEmpty(assertionName))
            {
                if (assertionName == null)
                {
                    throw new ArgumentNullException("assertionName");
                }
                throw new ArgumentException("cannot be null or empty", "assertionName");
            }
            HandleFail(assertionName, message ?? String.Empty, parameters);
        }

        public static void HandleFail(string assertionName, string message, params object[] parameters)
        {
            var t = typeof(Assert);
            var mi = t.GetTypeInfo().GetDeclaredMethod("HandleFail");
            try
            {
                mi.Invoke(null, new object[] { assertionName, message, parameters });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        public static string AppendMessage(this string source, string message, params object[] parameters)
        {
            var messageString = String.Format(CultureInfo.CurrentCulture, message, parameters);
            return String.Format(CultureInfo.CurrentCulture, "{0} {1}", messageString, source);
        }
    }
}
