using System;

namespace SeemplestBlocks.Testing.WinRtExtensions
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Asserts that this double value is greater than a certain value.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="comparer">The <i>double</i> to compare to.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBeGreaterThan(this double source, double comparer)
        {
            return source.ShouldBeGreaterThan(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this double value is greater than a certain value.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="comparer">The <i>double</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBeGreaterThan(this double source, double comparer, string message)
        {
            return source.ShouldBeGreaterThan(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this double value is greater than a certain value.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="comparer">The <i>double</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBeGreaterThan(this double source, double comparer, string message, params object[] parameters)
        {
            if (source <= comparer)
            {
                AssertHelper.HandleFail("ShouldBeGreaterThan", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this double value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="comparer">The <i>double</i> to compare to.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBeGreaterThanOrEqualTo(this double source, double comparer)
        {
            return source.ShouldBeGreaterThanOrEqualTo(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this double value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="comparer">The <i>double</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBeGreaterThanOrEqualTo(this double source, double comparer, string message)
        {
            return source.ShouldBeGreaterThanOrEqualTo(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this double value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="comparer">The <i>double</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBeGreaterThanOrEqualTo(this double source, double comparer, string message, params object[] parameters)
        {
            if (source < comparer)
            {
                AssertHelper.HandleFail("ShouldBeGreaterThanOrEqualTo", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this double value is less than a certain value.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="comparer">The <i>double</i> to compare to.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBeLessThan(this double source, double comparer)
        {
            return source.ShouldBeLessThan(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this double value is less than a certain value.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="comparer">The <i>double</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBeLessThan(this double source, double comparer, string message)
        {
            return source.ShouldBeLessThan(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this double value is less than a certain value.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="comparer">The <i>double</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBeLessThan(this double source, double comparer, string message, params object[] parameters)
        {
            if (source >= comparer)
            {
                AssertHelper.HandleFail("ShouldBeLessThan", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this double value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="comparer">The <i>double</i> to compare to.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBeLessThanOrEqualTo(this double source, double comparer)
        {
            return source.ShouldBeLessThanOrEqualTo(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this double value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="comparer">The <i>double</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBeLessThanOrEqualTo(this double source, double comparer, string message)
        {
            return source.ShouldBeLessThanOrEqualTo(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this double value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="comparer">The <i>double</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBeLessThanOrEqualTo(this double source, double comparer, string message, params object[] parameters)
        {
            if (source > comparer)
            {
                AssertHelper.HandleFail("ShouldBeLessThanOrEqualTo", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this double value is between two values.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>double</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>double</i> to be less than or equal to.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        public static double ShouldBeBetween(this double source, double lowerInclusive, double upperInclusive)
        {
            return source.ShouldBeBetween(lowerInclusive, upperInclusive, String.Empty);
        }

        /// <summary>
        /// Asserts that this double value is between two values.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>double</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>double</i> to be less than or equal to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        public static double ShouldBeBetween(this double source, double lowerInclusive, double upperInclusive, string message)
        {
            return source.ShouldBeBetween(lowerInclusive, upperInclusive, message, null);
        }

        /// <summary>
        /// Asserts that this double value is between two values.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>double</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>double</i> to be less than or equal to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        public static double ShouldBeBetween(this double source, double lowerInclusive, double upperInclusive, string message, params object[] parameters)
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
        /// Asserts that this double value is between two values.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>double</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>double</i> to be greater than.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        public static double ShouldNotBeBetween(this double source, double lowerExclusive, double upperExclusive)
        {
            return source.ShouldNotBeBetween(lowerExclusive, upperExclusive, String.Empty);
        }

        /// <summary>
        /// Asserts that this double value is between two values.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>double</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>double</i> to be greater than.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        public static double ShouldNotBeBetween(this double source, double lowerExclusive, double upperExclusive, string message)
        {
            return source.ShouldNotBeBetween(lowerExclusive, upperExclusive, message, null);
        }

        /// <summary>
        /// Asserts that this double value is between two values.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>double</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>double</i> to be greater than.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        public static double ShouldNotBeBetween(this double source, double lowerExclusive, double upperExclusive, string message, params object[] parameters)
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
        /// Asserts that this double value is positive or zero.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBePositive(this double source)
        {
            return source.ShouldBePositive(String.Empty);
        }

        /// <summary>
        /// Asserts that this double value is positive or zero.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBePositive(this double source, string message)
        {
            return source.ShouldBePositive(message, null);
        }

        /// <summary>
        /// Asserts that this double value is positive or zero.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBePositive(this double source, string message, params object[] parameters)
        {
            if (source < 0)
            {
                AssertHelper.HandleFail("ShouldBePositive", 
                    message.AppendMessage(Messages.ActualString, source), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this double value is negative.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBeNegative(this double source)
        {
            return source.ShouldBeNegative(String.Empty);
        }

        /// <summary>
        /// Asserts that this double value is negative.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBeNegative(this double source, string message)
        {
            return source.ShouldBeNegative(message, null);
        }

        /// <summary>
        /// Asserts that this double value is negative.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBeNegative(this double source, string message, params object[] parameters)
        {
            if (source >= 0)
            {
                AssertHelper.HandleFail("ShouldBeNegative", 
                    message.AppendMessage(Messages.ActualString, source), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this double value is integral (does not have a fractional component).
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBeIntegral(this double source)
        {
            return source.ShouldBeIntegral(String.Empty);
        }

        /// <summary>
        /// Asserts that this double value is integral (does not have a fractional component).
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBeIntegral(this double source, string message)
        {
            return source.ShouldBeIntegral(message, null);
        }

        /// <summary>
        /// Asserts that this double value is integral (does not have a fractional component).
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBeIntegral(this double source, string message, params object[] parameters)
        {
            if (Math.Abs(source - Math.Truncate(source)) > Double.Epsilon)
            {
                AssertHelper.HandleFail("ShouldBeIntegral", 
                    message.AppendMessage(Messages.ActualString, source), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this double value is fractional.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBeFractional(this double source)
        {
            return source.ShouldBeFractional(String.Empty);
        }

        /// <summary>
        /// Asserts that this double value is fractional.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBeFractional(this double source, string message)
        {
            return source.ShouldBeFractional(message, null);
        }

        /// <summary>
        /// Asserts that this double value is fractional.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldBeFractional(this double source, string message, params object[] parameters)
        {
            if (Math.Abs(source - Math.Truncate(source)) < Double.Epsilon)
            {
                AssertHelper.HandleFail("ShouldBeFractional", 
                    message.AppendMessage(Messages.ActualString, source), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this double value rounds to a specified value.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="comparer">The <i>long</i> to compare to.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldRoundTo(this double source, long comparer)
        {
            return source.ShouldRoundTo(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this double value rounds to a specified value.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="comparer">The <i>long</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldRoundTo(this double source, long comparer, string message)
        {
            return source.ShouldRoundTo(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this double value rounds to a specified value.
        /// </summary>
        /// <param name="source">The double value to test.</param>
        /// <param name="comparer">The <i>long</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>double</i> that was tested.</returns>
        public static double ShouldRoundTo(this double source, long comparer, string message, params object[] parameters)
        {
            var rounded = Math.Round(source);
            if (Math.Abs(rounded - (double) comparer) > Double.Epsilon)
            {
                AssertHelper.HandleFail("ShouldRoundTo", 
                    message.AppendMessage(Messages.ActualExpectedString, rounded, comparer), parameters);
            }

            return source;
        }
    }
}
