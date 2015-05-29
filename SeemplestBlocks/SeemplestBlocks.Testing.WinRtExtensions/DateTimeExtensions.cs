using System;

namespace SeemplestBlocks.Testing.WinRtExtensions
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Asserts that this DateTime value is before the specified DateTime value.
        /// </summary>
        /// <param name="source">The <i>DateTime</i> value to test.</param>
        /// <param name="comparer">The <i>DateTime</i> to compare to.</param>
        /// <returns>The original <i>DateTime</i> that was tested.</returns>
        public static DateTime ShouldBeBefore(this DateTime source, DateTime comparer)
        {
            return source.ShouldBeBefore(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this DateTime value is before the specified DateTime value.
        /// </summary>
        /// <param name="source">The <i>DateTime</i> value to test.</param>
        /// <param name="comparer">The <i>DateTime</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>DateTime</i> that was tested.</returns>
        public static DateTime ShouldBeBefore(this DateTime source, DateTime comparer, string message)
        {
            return source.ShouldBeBefore(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this DateTime value is before the specified DateTime value.
        /// </summary>
        /// <param name="source">The <i>DateTime</i> value to test.</param>
        /// <param name="comparer">The <i>DateTime</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>DateTime</i> that was tested.</returns>
        public static DateTime ShouldBeBefore(this DateTime source, DateTime comparer, string message, params object[] parameters)
        {
            if (source >= comparer)
            {
                AssertHelper.HandleFail("ShouldBeBefore", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this DateTime value is before or the same as the specified DateTime value.
        /// </summary>
        /// <param name="source">The <i>DateTime</i> value to test.</param>
        /// <param name="comparer">The <i>DateTime</i> to compare to.</param>
        /// <returns>The original <i>DateTime</i> that was tested.</returns>
        public static DateTime ShouldBeBeforeOrSameAs(this DateTime source, DateTime comparer)
        {
            return source.ShouldBeBeforeOrSameAs(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this DateTime value is before or the same as the specified DateTime value.
        /// </summary>
        /// <param name="source">The <i>DateTime</i> value to test.</param>
        /// <param name="comparer">The <i>DateTime</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>DateTime</i> that was tested.</returns>
        public static DateTime ShouldBeBeforeOrSameAs(this DateTime source, DateTime comparer, string message)
        {
            return source.ShouldBeBeforeOrSameAs(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this DateTime value is before or the same as the specified DateTime value.
        /// </summary>
        /// <param name="source">The <i>DateTime</i> value to test.</param>
        /// <param name="comparer">The <i>DateTime</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>DateTime</i> that was tested.</returns>
        public static DateTime ShouldBeBeforeOrSameAs(this DateTime source, DateTime comparer, string message, params object[] parameters)
        {
            if (source > comparer)
            {
                AssertHelper.HandleFail("ShouldBeBeforeOrSameAs", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this DateTime value is after the specified DateTime value.
        /// </summary>
        /// <param name="source">The <i>DateTime</i> value to test.</param>
        /// <param name="comparer">The <i>DateTime</i> to compare to.</param>
        /// <returns>The original <i>DateTime</i> that was tested.</returns>
        public static DateTime ShouldBeAfter(this DateTime source, DateTime comparer)
        {
            return source.ShouldBeAfter(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this DateTime value is after the specified DateTime value.
        /// </summary>
        /// <param name="source">The <i>DateTime</i> value to test.</param>
        /// <param name="comparer">The <i>DateTime</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>DateTime</i> that was tested.</returns>
        public static DateTime ShouldBeAfter(this DateTime source, DateTime comparer, string message)
        {
            return source.ShouldBeAfter(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this DateTime value is after the specified DateTime value.
        /// </summary>
        /// <param name="source">The <i>DateTime</i> value to test.</param>
        /// <param name="comparer">The <i>DateTime</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>DateTime</i> that was tested.</returns>
        public static DateTime ShouldBeAfter(this DateTime source, DateTime comparer, string message, params object[] parameters)
        {
            if (source <= comparer)
            {
                AssertHelper.HandleFail("ShouldBeAfter", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this DateTime value is after or the same as the specified DateTime value.
        /// </summary>
        /// <param name="source">The <i>DateTime</i> value to test.</param>
        /// <param name="comparer">The <i>DateTime</i> to compare to.</param>
        /// <returns>The original <i>DateTime</i> that was tested.</returns>
        public static DateTime ShouldBeAfterOrSameAs(this DateTime source, DateTime comparer)
        {
            return source.ShouldBeAfterOrSameAs(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this DateTime value is after or the same as the specified DateTime value.
        /// </summary>
        /// <param name="source">The <i>DateTime</i> value to test.</param>
        /// <param name="comparer">The <i>DateTime</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>DateTime</i> that was tested.</returns>
        public static DateTime ShouldBeAfterOrSameAs(this DateTime source, DateTime comparer, string message)
        {
            return source.ShouldBeAfterOrSameAs(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this DateTime value is after or the same as the specified DateTime value.
        /// </summary>
        /// <param name="source">The <i>DateTime</i> value to test.</param>
        /// <param name="comparer">The <i>DateTime</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>DateTime</i> that was tested.</returns>
        public static DateTime ShouldBeAfterOrSameAs(this DateTime source, DateTime comparer, string message, params object[] parameters)
        {
            if (source < comparer)
            {
                AssertHelper.HandleFail("ShouldBeAfterOrSameAs", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }
    }
}
