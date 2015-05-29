using System;
using System.Globalization;

namespace SeemplestBlocks.Testing.WinRtExtensions
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Asserts that this char value is equal to the specified char (ignoring case).
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="comparer">The <i>char</i> to compare to.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldEqualIgnoringCase(this char source, char comparer)
        {
            return source.ShouldEqualIgnoringCase(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this char value is equal to the specified char (ignoring case).
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="comparer">The <i>char</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldEqualIgnoringCase(this char source, char comparer, string message)
        {
            return source.ShouldEqualIgnoringCase(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this char value is equal to the specified char (ignoring case).
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="comparer">The <i>char</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldEqualIgnoringCase(this char source, char comparer, string message, params object[] parameters)
        {
            if (Char.ToUpper(source) != Char.ToUpper(comparer))
            {
                AssertHelper.HandleFail("ShouldEqualIgnoringCase", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this char value is not equal to the specified char (ignoring case).
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="comparer">The <i>char</i> to compare to.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldNotEqualIgnoringCase(this char source, char comparer)
        {
            return source.ShouldNotEqualIgnoringCase(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this char value is not equal to the specified char (ignoring case).
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="comparer">The <i>char</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldNotEqualIgnoringCase(this char source, char comparer, string message)
        {
            return source.ShouldNotEqualIgnoringCase(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this char value is not equal to the specified char (ignoring case).
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="comparer">The <i>char</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldNotEqualIgnoringCase(this char source, char comparer, string message, params object[] parameters)
        {
            if (Char.ToUpper(source) == Char.ToUpper(comparer))
            {
                AssertHelper.HandleFail("ShouldNotEqualIgnoringCase", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this char value is greater than a certain value.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="comparer">The <i>char</i> to compare to.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldBeGreaterThan(this char source, char comparer)
        {
            return source.ShouldBeGreaterThan(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this char value is greater than a certain value.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="comparer">The <i>char</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldBeGreaterThan(this char source, char comparer, string message)
        {
            return source.ShouldBeGreaterThan(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this char value is greater than a certain value.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="comparer">The <i>char</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldBeGreaterThan(this char source, char comparer, string message, params object[] parameters)
        {
            if (source <= comparer)
            {
                AssertHelper.HandleFail("ShouldBeGreaterThan", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this char value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="comparer">The <i>char</i> to compare to.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldBeGreaterThanOrEqualTo(this char source, char comparer)
        {
            return source.ShouldBeGreaterThanOrEqualTo(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this char value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="comparer">The <i>char</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldBeGreaterThanOrEqualTo(this char source, char comparer, string message)
        {
            return source.ShouldBeGreaterThanOrEqualTo(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this char value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="comparer">The <i>char</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldBeGreaterThanOrEqualTo(this char source, char comparer, string message, params object[] parameters)
        {
            if (source < comparer)
            {
                AssertHelper.HandleFail("ShouldBeGreaterThanOrEqualTo", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this char value is less than a certain value.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="comparer">The <i>char</i> to compare to.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldBeLessThan(this char source, char comparer)
        {
            return source.ShouldBeLessThan(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this char value is less than a certain value.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="comparer">The <i>char</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldBeLessThan(this char source, char comparer, string message)
        {
            return source.ShouldBeLessThan(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this char value is less than a certain value.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="comparer">The <i>char</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldBeLessThan(this char source, char comparer, string message, params object[] parameters)
        {
            if (source >= comparer)
            {
                AssertHelper.HandleFail("ShouldBeLessThan", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this char value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="comparer">The <i>char</i> to compare to.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldBeLessThanOrEqualTo(this char source, char comparer)
        {
            return source.ShouldBeLessThanOrEqualTo(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this char value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="comparer">The <i>char</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldBeLessThanOrEqualTo(this char source, char comparer, string message)
        {
            return source.ShouldBeLessThanOrEqualTo(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this char value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="comparer">The <i>char</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldBeLessThanOrEqualTo(this char source, char comparer, string message, params object[] parameters)
        {
            if (source > comparer)
            {
                AssertHelper.HandleFail("ShouldBeLessThanOrEqualTo", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this char value is between two values.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>char</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>char</i> to be less than or equal to.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        public static char ShouldBeBetween(this char source, char lowerInclusive, char upperInclusive)
        {
            return source.ShouldBeBetween(lowerInclusive, upperInclusive, String.Empty);
        }

        /// <summary>
        /// Asserts that this char value is between two values.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>char</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>char</i> to be less than or equal to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        public static char ShouldBeBetween(this char source, char lowerInclusive, char upperInclusive, string message)
        {
            return source.ShouldBeBetween(lowerInclusive, upperInclusive, message, null);
        }

        /// <summary>
        /// Asserts that this char value is between two values.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>char</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>char</i> to be less than or equal to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        public static char ShouldBeBetween(this char source, char lowerInclusive, char upperInclusive, string message, params object[] parameters)
        {
            if (lowerInclusive >= upperInclusive)
            {
                throw new ArgumentOutOfRangeException("lowerInclusive", 
                    Messages.ArgumentOutOfRangeExceptionLowerNotLessThanUpperMessage);
            }
            if (source < lowerInclusive || source > upperInclusive)
            {
                AssertHelper.HandleFail("ShouldBeBetween", 
                    message.AppendMessage(Messages.ActualExpectedRangeString, source, lowerInclusive, upperInclusive), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this char value is between two values.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>char</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>char</i> to be greater than.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        public static char ShouldNotBeBetween(this char source, char lowerExclusive, char upperExclusive)
        {
            return source.ShouldNotBeBetween(lowerExclusive, upperExclusive, String.Empty);
        }

        /// <summary>
        /// Asserts that this char value is between two values.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>char</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>char</i> to be greater than.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        public static char ShouldNotBeBetween(this char source, char lowerExclusive, char upperExclusive, string message)
        {
            return source.ShouldNotBeBetween(lowerExclusive, upperExclusive, message, null);
        }

        /// <summary>
        /// Asserts that this char value is between two values.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>char</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>char</i> to be greater than.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        public static char ShouldNotBeBetween(this char source, char lowerExclusive, char upperExclusive, string message, params object[] parameters)
        {
            if (lowerExclusive >= upperExclusive)
            {
                throw new ArgumentOutOfRangeException("lowerExclusive", 
                    Messages.ArgumentOutOfRangeExceptionLowerNotLessThanUpperMessage);
            }
            if (source >= lowerExclusive && source <= upperExclusive)
            {
                AssertHelper.HandleFail("ShouldNotBeBetween", 
                    message.AppendMessage(Messages.ActualExpectedRangeString, source, lowerExclusive, upperExclusive), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this char value is in the printable range.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldBePrintable(this char source)
        {
            return source.ShouldBePrintable(String.Empty);
        }

        /// <summary>
        /// Asserts that this char value is in the printable range.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldBePrintable(this char source, string message)
        {
            return source.ShouldBePrintable(message, null);
        }

        /// <summary>
        /// Asserts that this char value is in the printable range.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldBePrintable(this char source, string message, params object[] parameters)
        {
            if (source < 32 || source > 126)
            {
                AssertHelper.HandleFail("ShouldBePrintable",
                    message.AppendMessage(Messages.ActualString, String.Format(CultureInfo.CurrentCulture, "0x{0:X2}({0})", (byte)source)),
                    parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this char value is not in the printable range.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldNotBePrintable(this char source)
        {
            return source.ShouldNotBePrintable(String.Empty);
        }

        /// <summary>
        /// Asserts that this char value is not in the printable range.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldNotBePrintable(this char source, string message)
        {
            return source.ShouldNotBePrintable(message, null);
        }

        /// <summary>
        /// Asserts that this char value is not in the printable range.
        /// </summary>
        /// <param name="source">The char value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>char</i> that was tested.</returns>
        public static char ShouldNotBePrintable(this char source, string message, params object[] parameters)
        {
            if (source >= 32 && source <= 126)
            {
                AssertHelper.HandleFail("ShouldNotBePrintable", 
                    message.AppendMessage(Messages.ActualString, source), parameters);
            }

            return source;
        }
    }
}
