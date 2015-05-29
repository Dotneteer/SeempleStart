using System;

namespace SeemplestBlocks.Testing.WinRtExtensions
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Asserts that this object is equal to another object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <param name="comparer">The <i>object</i> to compare to.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        /// <remarks>This assertion can behave differently than expected if the object has overridden the Equals() method.</remarks>
        public static T ShouldEqual<T>(this T source, T comparer)
        {
            return source.ShouldEqual(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this object is equal to another object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <param name="comparer">The <i>object</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        /// <remarks>This assertion can behave differently than expected if the object has overridden the Equals() method.</remarks>
        public static T ShouldEqual<T>(this T source, T comparer, string message)
        {
            return source.ShouldEqual(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this object is equal to another object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <param name="comparer">The <i>object</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        /// <remarks>This assertion can behave differently than expected if the object has overridden the Equals() method.</remarks>
        public static T ShouldEqual<T>(this T source, T comparer, string message, params object[] parameters)
        {
            if (!Equals(source, comparer))
            {
                AssertHelper.HandleFail("ShouldEqual", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this object is not equal to another object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <param name="comparer">The <i>object</i> to compare to.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        /// <remarks>This assertion can behave differently than expected if the object has overridden the Equals() method.</remarks>
        public static T ShouldNotEqual<T>(this T source, T comparer)
        {
            return source.ShouldNotEqual(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this object is not equal to another object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <param name="comparer">The <i>object</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        /// <remarks>This assertion can behave differently than expected if the object has overridden the Equals() method.</remarks>
        public static T ShouldNotEqual<T>(this T source, T comparer, string message)
        {
            return source.ShouldNotEqual(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this object is not equal to another object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <param name="comparer">The <i>object</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        /// <remarks>This assertion can behave differently than expected if the object has overridden the Equals() method.</remarks>
        public static T ShouldNotEqual<T>(this T source, T comparer, string message, params object[] parameters)
        {
            if (Equals(source, comparer))
            {
                AssertHelper.HandleFail("ShouldNotEqual", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this object has the same reference as another object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <param name="comparer">The <i>object</i> to compare to.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        public static T ShouldBeSameAs<T>(this T source, T comparer) where T : class
        {
            return source.ShouldBeSameAs(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this object has the same reference as another object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <param name="comparer">The <i>object</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        public static T ShouldBeSameAs<T>(this T source, T comparer, string message) where T : class
        {
            return source.ShouldBeSameAs(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this object has the same reference as another object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <param name="comparer">The <i>object</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        public static T ShouldBeSameAs<T>(this T source, T comparer, string message, params object[] parameters) where T : class
        {
            if (!ReferenceEquals(source, comparer))
            {
                AssertHelper.HandleFail("ShouldBeSameAs", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this object does not have the same reference as another object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <param name="comparer">The <i>object</i> to compare to.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        public static T ShouldNotBeSameAs<T>(this T source, T comparer) where T : class
        {
            return source.ShouldNotBeSameAs(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this object does not have the same reference as another object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <param name="comparer">The <i>object</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        public static T ShouldNotBeSameAs<T>(this T source, T comparer, string message) where T : class
        {
            return source.ShouldNotBeSameAs(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this object does not have the same reference as another object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The object to test.</param>
        /// <param name="comparer">The <i>object</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>object</i> that was tested.</returns>
        public static T ShouldNotBeSameAs<T>(this T source, T comparer, string message, params object[] parameters) where T : class
        {
            if (ReferenceEquals(source, comparer))
            {
                AssertHelper.HandleFail("ShouldNotBeSameAs", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }
    }
}
