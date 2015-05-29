using System;
using System.Reflection;

namespace SeemplestBlocks.Testing.WinRtExtensions
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Asserts that this type value is assignable from another type.
        /// </summary>
        /// <typeparam name="TComparer">The <i>Type</i> to check for assignment from.</typeparam>
        /// <param name="source">The type value to test.</param>
        /// <returns>The original <i>Type</i> that was tested.</returns>
        public static Type ShouldBeAssignableFrom<TComparer>(this Type source)
        {
            return source.ShouldBeAssignableFrom(typeof(TComparer));
        }

        /// <summary>
        /// Asserts that this type value is assignable from another type.
        /// </summary>
        /// <typeparam name="TComparer">The <i>Type</i> to check for assignment from.</typeparam>
        /// <param name="source">The type value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>Type</i> that was tested.</returns>
        public static Type ShouldBeAssignableFrom<TComparer>(this Type source, string message)
        {
            return source.ShouldBeAssignableFrom(typeof(TComparer), message);
        }

        /// <summary>
        /// Asserts that this type value is assignable from another type.
        /// </summary>
        /// <typeparam name="TComparer">The <i>Type</i> to check for assignment from.</typeparam>
        /// <param name="source">The type value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>Type</i> that was tested.</returns>
        public static Type ShouldBeAssignableFrom<TComparer>(this Type source, string message, params object[] parameters)
        {
            return source.ShouldBeAssignableFrom(typeof(TComparer), message, parameters);
        }

        /// <summary>
        /// Asserts that this type value is assignable from another type.
        /// </summary>
        /// <param name="source">The type value to test.</param>
        /// <param name="comparer">The <i>Type</i> to check for assignment from.</param>
        /// <returns>The original <i>Type</i> that was tested.</returns>
        public static Type ShouldBeAssignableFrom(this Type source, Type comparer)
        {
            return source.ShouldBeAssignableFrom(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this type value is assignable from another type.
        /// </summary>
        /// <param name="source">The type value to test.</param>
        /// <param name="comparer">The <i>Type</i> to check for assignment from.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>Type</i> that was tested.</returns>
        public static Type ShouldBeAssignableFrom(this Type source, Type comparer, string message)
        {
            return source.ShouldBeAssignableFrom(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this type value is assignable from another type.
        /// </summary>
        /// <param name="source">The type value to test.</param>
        /// <param name="comparer">The <i>Type</i> to check for assignment from.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>Type</i> that was tested.</returns>
        public static Type ShouldBeAssignableFrom(this Type source, Type comparer, string message, params object[] parameters)
        {
            if (source.GetTypeInfo().IsAssignableFrom(comparer.GetTypeInfo()))
            {
                AssertHelper.HandleFail("ShouldBeAssignableFrom", 
                    message.AppendMessage(Messages.NotAssignableFromString, source, comparer), parameters);
            }

            return source;
        }
    }
}
