using System;

namespace SeemplestBlocks.Testing.WinRtExtensions
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Asserts that this decimal value is greater than a certain value.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="comparer">The <i>decimal</i> to compare to.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBeGreaterThan(this decimal source, decimal comparer)
        {
            return source.ShouldBeGreaterThan(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this decimal value is greater than a certain value.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="comparer">The <i>decimal</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBeGreaterThan(this decimal source, decimal comparer, string message)
        {
            return source.ShouldBeGreaterThan(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this decimal value is greater than a certain value.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="comparer">The <i>decimal</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBeGreaterThan(this decimal source, decimal comparer, string message, params object[] parameters)
        {
            if (source <= comparer)
            {
                AssertHelper.HandleFail("ShouldBeGreaterThan", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this decimal value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="comparer">The <i>decimal</i> to compare to.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBeGreaterThanOrEqualTo(this decimal source, decimal comparer)
        {
            return source.ShouldBeGreaterThanOrEqualTo(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this decimal value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="comparer">The <i>decimal</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBeGreaterThanOrEqualTo(this decimal source, decimal comparer, string message)
        {
            return source.ShouldBeGreaterThanOrEqualTo(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this decimal value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="comparer">The <i>decimal</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBeGreaterThanOrEqualTo(this decimal source, decimal comparer, string message, params object[] parameters)
        {
            if (source < comparer)
            {
                AssertHelper.HandleFail("ShouldBeGreaterThanOrEqualTo", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this decimal value is less than a certain value.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="comparer">The <i>decimal</i> to compare to.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBeLessThan(this decimal source, decimal comparer)
        {
            return source.ShouldBeLessThan(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this decimal value is less than a certain value.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="comparer">The <i>decimal</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBeLessThan(this decimal source, decimal comparer, string message)
        {
            return source.ShouldBeLessThan(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this decimal value is less than a certain value.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="comparer">The <i>decimal</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBeLessThan(this decimal source, decimal comparer, string message, params object[] parameters)
        {
            if (source >= comparer)
            {
                AssertHelper.HandleFail("ShouldBeLessThan", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this decimal value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="comparer">The <i>decimal</i> to compare to.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBeLessThanOrEqualTo(this decimal source, decimal comparer)
        {
            return source.ShouldBeLessThanOrEqualTo(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this decimal value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="comparer">The <i>decimal</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBeLessThanOrEqualTo(this decimal source, decimal comparer, string message)
        {
            return source.ShouldBeLessThanOrEqualTo(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this decimal value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="comparer">The <i>decimal</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBeLessThanOrEqualTo(this decimal source, decimal comparer, string message, params object[] parameters)
        {
            if (source > comparer)
            {
                AssertHelper.HandleFail("ShouldBeLessThanOrEqualTo", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this decimal value is between two values.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>decimal</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>decimal</i> to be less than or equal to.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        public static decimal ShouldBeBetween(this decimal source, decimal lowerInclusive, decimal upperInclusive)
        {
            return source.ShouldBeBetween(lowerInclusive, upperInclusive, String.Empty);
        }

        /// <summary>
        /// Asserts that this decimal value is between two values.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>decimal</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>decimal</i> to be less than or equal to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        public static decimal ShouldBeBetween(this decimal source, decimal lowerInclusive, decimal upperInclusive, string message)
        {
            return source.ShouldBeBetween(lowerInclusive, upperInclusive, message, null);
        }

        /// <summary>
        /// Asserts that this decimal value is between two values.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>decimal</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>decimal</i> to be less than or equal to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        public static decimal ShouldBeBetween(this decimal source, decimal lowerInclusive, decimal upperInclusive, string message, params object[] parameters)
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
        /// Asserts that this decimal value is between two values.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>decimal</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>decimal</i> to be greater than.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        public static decimal ShouldNotBeBetween(this decimal source, decimal lowerExclusive, decimal upperExclusive)
        {
            return source.ShouldNotBeBetween(lowerExclusive, upperExclusive, String.Empty);
        }

        /// <summary>
        /// Asserts that this decimal value is between two values.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>decimal</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>decimal</i> to be greater than.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        public static decimal ShouldNotBeBetween(this decimal source, decimal lowerExclusive, decimal upperExclusive, string message)
        {
            return source.ShouldNotBeBetween(lowerExclusive, upperExclusive, message, null);
        }

        /// <summary>
        /// Asserts that this decimal value is between two values.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>decimal</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>decimal</i> to be greater than.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        public static decimal ShouldNotBeBetween(this decimal source, decimal lowerExclusive, decimal upperExclusive, string message, params object[] parameters)
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
        /// Asserts that this decimal value is positive or zero.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBePositive(this decimal source)
        {
            return source.ShouldBePositive(String.Empty);
        }

        /// <summary>
        /// Asserts that this decimal value is positive or zero.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBePositive(this decimal source, string message)
        {
            return source.ShouldBePositive(message, null);
        }

        /// <summary>
        /// Asserts that this decimal value is positive or zero.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBePositive(this decimal source, string message, params object[] parameters)
        {
            if (source < 0)
            {
                AssertHelper.HandleFail("ShouldBePositive", 
                    message.AppendMessage(Messages.ActualString, source), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this decimal value is negative.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBeNegative(this decimal source)
        {
            return source.ShouldBeNegative(String.Empty);
        }

        /// <summary>
        /// Asserts that this decimal value is negative.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBeNegative(this decimal source, string message)
        {
            return source.ShouldBeNegative(message, null);
        }

        /// <summary>
        /// Asserts that this decimal value is negative.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBeNegative(this decimal source, string message, params object[] parameters)
        {
            if (source >= 0)
            {
                AssertHelper.HandleFail("ShouldBeNegative", 
                    message.AppendMessage(Messages.ActualString, source), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this decimal value is integral (does not have a fractional component).
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBeIntegral(this decimal source)
        {
            return source.ShouldBeIntegral(String.Empty);
        }

        /// <summary>
        /// Asserts that this decimal value is integral (does not have a fractional component).
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBeIntegral(this decimal source, string message)
        {
            return source.ShouldBeIntegral(message, null);
        }

        /// <summary>
        /// Asserts that this decimal value is integral (does not have a fractional component).
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBeIntegral(this decimal source, string message, params object[] parameters)
        {
            if (source - Math.Truncate(source) != 0)
            {
                AssertHelper.HandleFail("ShouldBeIntegral", 
                    message.AppendMessage(Messages.ActualString, source), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this decimal value is fractional.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBeFractional(this decimal source)
        {
            return source.ShouldBeFractional(String.Empty);
        }

        /// <summary>
        /// Asserts that this decimal value is fractional.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBeFractional(this decimal source, string message)
        {
            return source.ShouldBeFractional(message, null);
        }

        /// <summary>
        /// Asserts that this decimal value is fractional.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldBeFractional(this decimal source, string message, params object[] parameters)
        {
            if (source - Math.Truncate(source) == 0)
            {
                AssertHelper.HandleFail("ShouldBeFractional", 
                    message.AppendMessage(Messages.ActualString, source), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this decimal value rounds to a specified value.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="comparer">The <i>long</i> to compare to.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldRoundTo(this decimal source, long comparer)
        {
            return source.ShouldRoundTo(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this decimal value rounds to a specified value.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="comparer">The <i>long</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldRoundTo(this decimal source, long comparer, string message)
        {
            return source.ShouldRoundTo(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this decimal value rounds to a specified value.
        /// </summary>
        /// <param name="source">The decimal value to test.</param>
        /// <param name="comparer">The <i>long</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>decimal</i> that was tested.</returns>
        public static decimal ShouldRoundTo(this decimal source, long comparer, string message, params object[] parameters)
        {
            var rounded = Math.Round(source);
            if (rounded != comparer)
            {
                AssertHelper.HandleFail("ShouldRoundTo", 
                    message.AppendMessage(Messages.ActualExpectedString, rounded, comparer), parameters);
            }

            return source;
        }
    }
}
