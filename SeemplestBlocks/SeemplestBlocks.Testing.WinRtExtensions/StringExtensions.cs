using System;
using System.Diagnostics.CodeAnalysis;

namespace SeemplestBlocks.Testing.WinRtExtensions
{
    public partial class ExtensionMethods
    {
        /// <summary>
        /// Asserts that this string value is equal to the specified string (ignoring case).
        /// </summary>
        /// <param name="source">The string value to test.</param>
        /// <param name="comparer">The <i>string</i> to compare to.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldEqualIgnoringCase(this string source, string comparer)
        {
            return source.ShouldEqualIgnoringCase(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this string value is equal to the specified string (ignoring case).
        /// </summary>
        /// <param name="source">The string value to test.</param>
        /// <param name="comparer">The <i>string</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldEqualIgnoringCase(this string source, string comparer, string message)
        {
            return source.ShouldEqualIgnoringCase(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this string value is equal to the specified string (ignoring case).
        /// </summary>
        /// <param name="source">The string value to test.</param>
        /// <param name="comparer">The <i>string</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldEqualIgnoringCase(this string source, string comparer, string message, params object[] parameters)
        {
            if (String.Compare(source, comparer, StringComparison.OrdinalIgnoreCase) != 0)
            {
                AssertHelper.HandleFail("ShouldEqualIgnoringCase", 
                    message.AppendMessage(Messages.ActualExpectedString, source ?? Messages.NullString, 
                    comparer ?? Messages.NullString), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this string value is not equal to the specified string (ignoring case).
        /// </summary>
        /// <param name="source">The string value to test.</param>
        /// <param name="comparer">The <i>string</i> to compare to.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldNotEqualIgnoringCase(this string source, string comparer)
        {
            return source.ShouldNotEqualIgnoringCase(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this string value is not equal to the specified string (ignoring case).
        /// </summary>
        /// <param name="source">The string value to test.</param>
        /// <param name="comparer">The <i>string</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldNotEqualIgnoringCase(this string source, string comparer, string message)
        {
            return source.ShouldNotEqualIgnoringCase(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this string value is not equal to the specified string (ignoring case).
        /// </summary>
        /// <param name="source">The string value to test.</param>
        /// <param name="comparer">The <i>string</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldNotEqualIgnoringCase(this string source, string comparer, string message, params object[] parameters)
        {
            if (String.Compare(source, comparer, StringComparison.OrdinalIgnoreCase) == 0)
            {
                AssertHelper.HandleFail("ShouldNotEqualIgnoringCase", 
                    message.AppendMessage(Messages.ActualExpectedString, source ?? Messages.NullString, 
                    comparer ?? Messages.NullString), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this string value is empty.
        /// </summary>
        /// <param name="source">The string value to test.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldBeEmpty(this string source)
        {
            return source.ShouldBeEmpty(String.Empty);
        }

        /// <summary>
        /// Asserts that this string value is empty.
        /// </summary>
        /// <param name="source">The string value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldBeEmpty(this string source, string message)
        {
            return source.ShouldBeEmpty(message, null);
        }

        /// <summary>
        /// Asserts that this string value is empty.
        /// </summary>
        /// <param name="source">The string value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength")]
        public static string ShouldBeEmpty(this string source, string message, params object[] parameters)
        {
            if (source != String.Empty)
            {
                AssertHelper.HandleFail("ShouldBeEmpty", 
                    message.AppendMessage(Messages.ActualString, source ?? Messages.NullString), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this string value is not empty.
        /// </summary>
        /// <param name="source">The string value to test.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldNotBeEmpty(this string source)
        {
            return source.ShouldNotBeEmpty(String.Empty);
        }

        /// <summary>
        /// Asserts that this string value is not empty.
        /// </summary>
        /// <param name="source">The string value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldNotBeEmpty(this string source, string message)
        {
            return source.ShouldNotBeEmpty(message, null);
        }

        /// <summary>
        /// Asserts that this string value is not empty.
        /// </summary>
        /// <param name="source">The string value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength")]
        public static string ShouldNotBeEmpty(this string source, string message, params object[] parameters)
        {
            if (source == String.Empty)
            {
                AssertHelper.HandleFail("ShouldNotBeEmpty", 
                    message.AppendMessage(Messages.ActualString, source ?? Messages.NullString), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this string value contains the specified substring.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="substring">The substring to test for containment.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldContain(this string source, string substring)
        {
            return source.ShouldContain(substring, String.Empty);
        }

        /// <summary>
        /// Asserts that this string value contains the specified substring.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="substring">The substring to test for containment.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldContain(this string source, string substring, string message)
        {
            return source.ShouldContain(substring, message, null);
        }

        /// <summary>
        /// Asserts that this string value contains the specified substring.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="substring">The substring to test for containment.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldContain(this string source, string substring, string message, params object[] parameters)
        {
            if (!source.Contains(substring))
            {
                AssertHelper.HandleFail("ShouldContain", 
                    message.AppendMessage(Messages.ActualExpectedContainString, source, 
                    substring ?? Messages.NullString), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this string value contains the specified substring, ignoring case.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="substring">The substring to test for containment.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldContainIgnoringCase(this string source, string substring)
        {
            return source.ShouldContainIgnoringCase(substring, String.Empty);
        }

        /// <summary>
        /// Asserts that this string value contains the specified substring, ignoring case.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="substring">The substring to test for containment.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldContainIgnoringCase(this string source, string substring, string message)
        {
            return source.ShouldContainIgnoringCase(substring, message, null);
        }

        /// <summary>
        /// Asserts that this string value contains the specified substring, ignoring case.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="substring">The substring to test for containment.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldContainIgnoringCase(this string source, string substring, string message, params object[] parameters)
        {
            if (!source.ToUpper().Contains(substring.ToUpper()))
            {
                AssertHelper.HandleFail("ShouldContainIgnoringCase", 
                    message.AppendMessage(Messages.ActualExpectedContainString, source, substring), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this string value does not contain the specified substring.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="substring">The substring to test for containment.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldNotContain(this string source, string substring)
        {
            return source.ShouldNotContain(substring, String.Empty);
        }

        /// <summary>
        /// Asserts that this string value does not contain the specified substring.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="substring">The substring to test for containment.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldNotContain(this string source, string substring, string message)
        {
            return source.ShouldNotContain(substring, message, null);
        }

        /// <summary>
        /// Asserts that this string value does not contain the specified substring.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="substring">The substring to test for containment.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldNotContain(this string source, string substring, string message, params object[] parameters)
        {
            if (source.Contains(substring))
            {
                AssertHelper.HandleFail("ShouldNotContain", 
                    message.AppendMessage(Messages.ActualExpectedContainString, source, substring ?? Messages.NullString), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this string value does not contain the specified substring, ignoring case.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="substring">The substring to test for containment.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldNotContainIgnoringCase(this string source, string substring)
        {
            return source.ShouldNotContainIgnoringCase(substring, String.Empty);
        }

        /// <summary>
        /// Asserts that this string value does not contain the specified substring, ignoring case.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="substring">The substring to test for containment.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldNotContainIgnoringCase(this string source, string substring, string message)
        {
            return source.ShouldNotContainIgnoringCase(substring, message, null);
        }

        /// <summary>
        /// Asserts that this string value does not contain the specified substring, ignoring case.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="substring">The substring to test for containment.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldNotContainIgnoringCase(this string source, string substring, string message, params object[] parameters)
        {
            if (source.ToUpper().Contains(substring.ToUpper()))
            {
                AssertHelper.HandleFail("ShouldNotContainIgnoringCase", 
                    message.AppendMessage(Messages.ActualExpectedContainString, source, substring), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this string value has a specified length.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="length">The expected length of the string.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldHaveLengthOf(this string source, int length)
        {
            return source.ShouldHaveLengthOf(length, String.Empty);
        }

        /// <summary>
        /// Asserts that this string value has a specified length.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="length">The expected length of the string.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldHaveLengthOf(this string source, int length, string message)
        {
            return source.ShouldHaveLengthOf(length, message, null);
        }

        /// <summary>
        /// Asserts that this string value has a specified length.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="length">The expected length of the string.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldHaveLengthOf(this string source, int length, string message, params object[] parameters)
        {
            if (source.Length != length)
            {
                AssertHelper.HandleFail("ShouldHaveLengthOf", 
                    message.AppendMessage(Messages.ActualExpectedString, source.Length.ToString(), length), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this string value does not have a specified length.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="length">The length that the string should not be.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldNotHaveLengthOf(this string source, int length)
        {
            return source.ShouldNotHaveLengthOf(length, String.Empty);
        }

        /// <summary>
        /// Asserts that this string value does not have a specified length.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="length">The length that the string should not be.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldNotHaveLengthOf(this string source, int length, string message)
        {
            return source.ShouldNotHaveLengthOf(length, message, null);
        }

        /// <summary>
        /// Asserts that this string value does not have a specified length.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="length">The length that the string should not be.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldNotHaveLengthOf(this string source, int length, string message, params object[] parameters)
        {
            if (source.Length == length)
            {
                AssertHelper.HandleFail("ShouldNotHaveLengthOf", 
                    message.AppendMessage(Messages.ActualExpectedString, source.Length.ToString(), length), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this string value has a minimum specified length.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="length">The minimum expected length.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldHaveLengthOfAtLeast(this string source, int length)
        {
            return source.ShouldHaveLengthOfAtLeast(length, String.Empty);
        }

        /// <summary>
        /// Asserts that this string value has a minimum specified length.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="length">The minimum expected length.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldHaveLengthOfAtLeast(this string source, int length, string message)
        {
            return source.ShouldHaveLengthOfAtLeast(length, message, null);
        }

        /// <summary>
        /// Asserts that this string value has a minimum specified length.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="length">The minimum expected length.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldHaveLengthOfAtLeast(this string source, int length, string message, params object[] parameters)
        {
            if (source.Length < length)
            {
                AssertHelper.HandleFail("ShouldHaveLengthOfAtLeast", 
                    message.AppendMessage(Messages.ActualExpectedString, source.Length.ToString(), length), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this string value has a maximum specified length.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="length">The maximum expected length.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldHaveLengthOfAtMost(this string source, int length)
        {
            return source.ShouldHaveLengthOfAtMost(length, String.Empty);
        }

        /// <summary>
        /// Asserts that this string value has a maximum specified length.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="length">The maximum expected length.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldHaveLengthOfAtMost(this string source, int length, string message)
        {
            return source.ShouldHaveLengthOfAtMost(length, message, null);
        }

        /// <summary>
        /// Asserts that this string value has a maximum specified length.
        /// </summary>
        /// <param name="source">This string value to test.</param>
        /// <param name="length">The maximum expected length.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>string</i> that was tested.</returns>
        public static string ShouldHaveLengthOfAtMost(this string source, int length, string message, params object[] parameters)
        {
            if (source.Length > length)
            {
                AssertHelper.HandleFail("ShouldHaveLengthOfAtMost", 
                    message.AppendMessage(Messages.ActualExpectedString, source.Length.ToString(), length), parameters);
            }

            return source;
        }
    }
}
