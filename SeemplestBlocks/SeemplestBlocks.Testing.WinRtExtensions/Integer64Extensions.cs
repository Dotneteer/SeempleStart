using System;

namespace SeemplestBlocks.Testing.WinRtExtensions
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Asserts that this long value is greater than a certain value.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <param name="comparer">The <i>long</i> to compare to.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        public static long ShouldBeGreaterThan(this long source, long comparer)
        {
            return source.ShouldBeGreaterThan(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this long value is greater than a certain value.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <param name="comparer">The <i>long</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        public static long ShouldBeGreaterThan(this long source, long comparer, string message)
        {
            return source.ShouldBeGreaterThan(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this long value is greater than a certain value.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <param name="comparer">The <i>long</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        public static long ShouldBeGreaterThan(this long source, long comparer, string message, params object[] parameters)
        {
            if (source <= comparer)
            {
                AssertHelper.HandleFail("ShouldBeGreaterThan", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this long value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <param name="comparer">The <i>long</i> to compare to.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        public static long ShouldBeGreaterThanOrEqualTo(this long source, long comparer)
        {
            return source.ShouldBeGreaterThanOrEqualTo(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this long value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <param name="comparer">The <i>long</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        public static long ShouldBeGreaterThanOrEqualTo(this long source, long comparer, string message)
        {
            return source.ShouldBeGreaterThanOrEqualTo(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this long value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <param name="comparer">The <i>long</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        public static long ShouldBeGreaterThanOrEqualTo(this long source, long comparer, string message, params object[] parameters)
        {
            if (source < comparer)
            {
                AssertHelper.HandleFail("ShouldBeGreaterThanOrEqualTo", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this long value is less than a certain value.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <param name="comparer">The <i>long</i> to compare to.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        public static long ShouldBeLessThan(this long source, long comparer)
        {
            return source.ShouldBeLessThan(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this long value is less than a certain value.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <param name="comparer">The <i>long</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        public static long ShouldBeLessThan(this long source, long comparer, string message)
        {
            return source.ShouldBeLessThan(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this long value is less than a certain value.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <param name="comparer">The <i>long</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        public static long ShouldBeLessThan(this long source, long comparer, string message, params object[] parameters)
        {
            if (source >= comparer)
            {
                AssertHelper.HandleFail("ShouldBeLessThan", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this long value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <param name="comparer">The <i>long</i> to compare to.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        public static long ShouldBeLessThanOrEqualTo(this long source, long comparer)
        {
            return source.ShouldBeLessThanOrEqualTo(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this long value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <param name="comparer">The <i>long</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        public static long ShouldBeLessThanOrEqualTo(this long source, long comparer, string message)
        {
            return source.ShouldBeLessThanOrEqualTo(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this long value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <param name="comparer">The <i>long</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        public static long ShouldBeLessThanOrEqualTo(this long source, long comparer, string message, params object[] parameters)
        {
            if (source > comparer)
            {
                AssertHelper.HandleFail("ShouldBeLessThanOrEqualTo", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this long value is between two values.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>long</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>long</i> to be less than or equal to.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        public static long ShouldBeBetween(this long source, long lowerInclusive, long upperInclusive)
        {
            return source.ShouldBeBetween(lowerInclusive, upperInclusive, String.Empty);
        }

        /// <summary>
        /// Asserts that this long value is between two values.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>long</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>long</i> to be less than or equal to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        public static long ShouldBeBetween(this long source, long lowerInclusive, long upperInclusive, string message)
        {
            return source.ShouldBeBetween(lowerInclusive, upperInclusive, message, null);
        }

        /// <summary>
        /// Asserts that this long value is between two values.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>long</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>long</i> to be less than or equal to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        public static long ShouldBeBetween(this long source, long lowerInclusive, long upperInclusive, string message, params object[] parameters)
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
        /// Asserts that this long value is between two values.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>long</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>long</i> to be greater than.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        public static long ShouldNotBeBetween(this long source, long lowerExclusive, long upperExclusive)
        {
            return source.ShouldNotBeBetween(lowerExclusive, upperExclusive, String.Empty);
        }

        /// <summary>
        /// Asserts that this long value is between two values.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>long</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>long</i> to be greater than.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        public static long ShouldNotBeBetween(this long source, long lowerExclusive, long upperExclusive, string message)
        {
            return source.ShouldNotBeBetween(lowerExclusive, upperExclusive, message, null);
        }

        /// <summary>
        /// Asserts that this long value is between two values.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>long</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>long</i> to be greater than.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        public static long ShouldNotBeBetween(this long source, long lowerExclusive, long upperExclusive, string message, params object[] parameters)
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
        /// Asserts that this long value is positive or zero.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        public static long ShouldBePositive(this long source)
        {
            return source.ShouldBePositive(String.Empty);
        }

        /// <summary>
        /// Asserts that this long value is positive or zero.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        public static long ShouldBePositive(this long source, string message)
        {
            return source.ShouldBePositive(message, null);
        }

        /// <summary>
        /// Asserts that this long value is positive or zero.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        public static long ShouldBePositive(this long source, string message, params object[] parameters)
        {
            if (source < 0)
            {
                AssertHelper.HandleFail("ShouldBePositive", 
                    message.AppendMessage(Messages.ActualString, source), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this long value is negative.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        public static long ShouldBeNegative(this long source)
        {
            return source.ShouldBeNegative(String.Empty);
        }

        /// <summary>
        /// Asserts that this long value is negative.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        public static long ShouldBeNegative(this long source, string message)
        {
            return source.ShouldBeNegative(message, null);
        }

        /// <summary>
        /// Asserts that this long value is negative.
        /// </summary>
        /// <param name="source">The long value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>long</i> that was tested.</returns>
        public static long ShouldBeNegative(this long source, string message, params object[] parameters)
        {
            if (source >= 0)
            {
                AssertHelper.HandleFail("ShouldBeNegative", 
                    message.AppendMessage(Messages.ActualString, source), parameters);
            }

            return source;
        }
    }
}
