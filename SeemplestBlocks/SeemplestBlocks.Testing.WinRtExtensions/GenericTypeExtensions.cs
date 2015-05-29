using System;

namespace SeemplestBlocks.Testing.WinRtExtensions
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Asserts that this object is a certain type.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        public static T ShouldBeOfType<T>(this T source, Type type)
        {
            return source.ShouldBeOfType(type, String.Empty);
        }

        /// <summary>
        /// Asserts that this object is a certain type.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        public static T ShouldBeOfType<T>(this T source, Type type, string message)
        {
            return source.ShouldBeOfType(type, message, null);
        }

        /// <summary>
        /// Asserts that this object is a certain type.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        public static T ShouldBeOfType<T>(this T source, Type type, string message, params object[] parameters)
        {
            if (source.GetType() != type)
            {
                AssertHelper.HandleFail("ShouldBeOfType", 
                    message.AppendMessage(Messages.ActualExpectedString, source.GetType(), type), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this object is not a certain type.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        public static T ShouldNotBeOfType<T>(this T source, Type type)
        {
            return source.ShouldNotBeOfType(type, String.Empty);
        }

        /// <summary>
        /// Asserts that this object is not a certain type.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        public static T ShouldNotBeOfType<T>(this T source, Type type, string message)
        {
            return source.ShouldNotBeOfType(type, message, null);
        }

        /// <summary>
        /// Asserts that this object is not a certain type.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        public static T ShouldNotBeOfType<T>(this T source, Type type, string message, params object[] parameters)
        {
            if (source.GetType() == type)
            {
                AssertHelper.HandleFail("ShouldNotBeOfType", 
                    message.AppendMessage(Messages.ActualExpectedString, source.GetType(), type), parameters);
            }

            return source;
        }
    }
}
