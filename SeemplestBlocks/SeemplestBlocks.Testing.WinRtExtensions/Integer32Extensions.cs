using System;

namespace SeemplestBlocks.Testing.WinRtExtensions
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Asserts that this int value is greater than a certain value.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <param name="comparer">The <i>int</i> to compare to.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        public static int ShouldBeGreaterThan(this int source, int comparer)
        {
            return source.ShouldBeGreaterThan(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this int value is greater than a certain value.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <param name="comparer">The <i>int</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        public static int ShouldBeGreaterThan(this int source, int comparer, string message)
        {
            return source.ShouldBeGreaterThan(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this int value is greater than a certain value.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <param name="comparer">The <i>int</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        public static int ShouldBeGreaterThan(this int source, int comparer, string message, params object[] parameters)
        {
            if (source <= comparer)
            {
                AssertHelper.HandleFail("ShouldBeGreaterThan", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this int value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <param name="comparer">The <i>int</i> to compare to.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        public static int ShouldBeGreaterThanOrEqualTo(this int source, int comparer)
        {
            return source.ShouldBeGreaterThanOrEqualTo(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this int value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <param name="comparer">The <i>int</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        public static int ShouldBeGreaterThanOrEqualTo(this int source, int comparer, string message)
        {
            return source.ShouldBeGreaterThanOrEqualTo(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this int value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <param name="comparer">The <i>int</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        public static int ShouldBeGreaterThanOrEqualTo(this int source, int comparer, string message, params object[] parameters)
        {
            if (source < comparer)
            {
                AssertHelper.HandleFail("ShouldBeGreaterThanOrEqualTo", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this int value is less than a certain value.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <param name="comparer">The <i>int</i> to compare to.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        public static int ShouldBeLessThan(this int source, int comparer)
        {
            return source.ShouldBeLessThan(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this int value is less than a certain value.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <param name="comparer">The <i>int</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        public static int ShouldBeLessThan(this int source, int comparer, string message)
        {
            return source.ShouldBeLessThan(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this int value is less than a certain value.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <param name="comparer">The <i>int</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        public static int ShouldBeLessThan(this int source, int comparer, string message, params object[] parameters)
        {
            if (source >= comparer)
            {
                AssertHelper.HandleFail("ShouldBeLessThan", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this int value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <param name="comparer">The <i>int</i> to compare to.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        public static int ShouldBeLessThanOrEqualTo(this int source, int comparer)
        {
            return source.ShouldBeLessThanOrEqualTo(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this int value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <param name="comparer">The <i>int</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        public static int ShouldBeLessThanOrEqualTo(this int source, int comparer, string message)
        {
            return source.ShouldBeLessThanOrEqualTo(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this int value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <param name="comparer">The <i>int</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        public static int ShouldBeLessThanOrEqualTo(this int source, int comparer, string message, params object[] parameters)
        {
            if (source > comparer)
            {
                AssertHelper.HandleFail("ShouldBeLessThanOrEqualTo", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this int value is between two values.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>int</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>int</i> to be less than or equal to.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        public static int ShouldBeBetween(this int source, int lowerInclusive, int upperInclusive)
        {
            return source.ShouldBeBetween(lowerInclusive, upperInclusive, String.Empty);
        }

        /// <summary>
        /// Asserts that this int value is between two values.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>int</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>int</i> to be less than or equal to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        public static int ShouldBeBetween(this int source, int lowerInclusive, int upperInclusive, string message)
        {
            return source.ShouldBeBetween(lowerInclusive, upperInclusive, message, null);
        }

        /// <summary>
        /// Asserts that this int value is between two values.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>int</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>int</i> to be less than or equal to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        public static int ShouldBeBetween(this int source, int lowerInclusive, int upperInclusive, string message, params object[] parameters)
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
        /// Asserts that this int value is between two values.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>int</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>int</i> to be greater than.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        public static int ShouldNotBeBetween(this int source, int lowerExclusive, int upperExclusive)
        {
            return source.ShouldNotBeBetween(lowerExclusive, upperExclusive, String.Empty);
        }

        /// <summary>
        /// Asserts that this int value is between two values.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>int</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>int</i> to be greater than.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        public static int ShouldNotBeBetween(this int source, int lowerExclusive, int upperExclusive, string message)
        {
            return source.ShouldNotBeBetween(lowerExclusive, upperExclusive, message, null);
        }

        /// <summary>
        /// Asserts that this int value is between two values.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>int</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>int</i> to be greater than.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        public static int ShouldNotBeBetween(this int source, int lowerExclusive, int upperExclusive, string message, params object[] parameters)
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
        /// Asserts that this int value is positive or zero.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        public static int ShouldBePositive(this int source)
        {
            return source.ShouldBePositive(String.Empty);
        }

        /// <summary>
        /// Asserts that this int value is positive or zero.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        public static int ShouldBePositive(this int source, string message)
        {
            return source.ShouldBePositive(message, null);
        }

        /// <summary>
        /// Asserts that this int value is positive or zero.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        public static int ShouldBePositive(this int source, string message, params object[] parameters)
        {
            if (source < 0)
            {
                AssertHelper.HandleFail("ShouldBePositive", 
                    message.AppendMessage(Messages.ActualString, source), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this int value is negative.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        public static int ShouldBeNegative(this int source)
        {
            return source.ShouldBeNegative(String.Empty);
        }

        /// <summary>
        /// Asserts that this int value is negative.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        public static int ShouldBeNegative(this int source, string message)
        {
            return source.ShouldBeNegative(message, null);
        }

        /// <summary>
        /// Asserts that this int value is negative.
        /// </summary>
        /// <param name="source">The int value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>int</i> that was tested.</returns>
        public static int ShouldBeNegative(this int source, string message, params object[] parameters)
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
