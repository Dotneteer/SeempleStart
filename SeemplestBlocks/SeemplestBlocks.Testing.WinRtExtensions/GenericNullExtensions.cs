using System;

namespace SeemplestBlocks.Testing.WinRtExtensions
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Asserts that this object is null.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        public static T ShouldBeNull<T>(this T source)
        {
            return source.ShouldBeNull(String.Empty);
        }

        /// <summary>
        /// Asserts that this object is null.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        public static T ShouldBeNull<T>(this T source, string message)
        {
            return source.ShouldBeNull(message, null);
        }

        /// <summary>
        /// Asserts that this object is null.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        public static T ShouldBeNull<T>(this T source, string message, params object[] parameters)
        {
            if (source != null)
                AssertHelper.HandleFail("ShouldBeNull", message, parameters);

            return source;
        }

        /// <summary>
        /// Asserts that this object is not null.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        public static T ShouldNotBeNull<T>(this T source)
        {
            return source.ShouldNotBeNull(String.Empty);
        }

        /// <summary>
        /// Asserts that this object is not null.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        public static T ShouldNotBeNull<T>(this T source, string message)
        {
            return source.ShouldNotBeNull(message, null);
        }

        /// <summary>
        /// Asserts that this object is not null.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        public static T ShouldNotBeNull<T>(this T source, string message, params object[] parameters)
        {
            if (source == null)
                AssertHelper.HandleFail("ShouldNotBeNull", message, parameters);

            return source;
        }
    }
}
