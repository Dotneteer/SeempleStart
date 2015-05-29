using System;

namespace SeemplestBlocks.Testing.WinRtExtensions
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Asserts that this float value is greater than a certain value.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="comparer">The <i>float</i> to compare to.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBeGreaterThan(this float source, float comparer)
        {
            return source.ShouldBeGreaterThan(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this float value is greater than a certain value.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="comparer">The <i>float</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBeGreaterThan(this float source, float comparer, string message)
        {
            return source.ShouldBeGreaterThan(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this float value is greater than a certain value.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="comparer">The <i>float</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBeGreaterThan(this float source, float comparer, string message, params object[] parameters)
        {
            if (source <= comparer)
            {
                AssertHelper.HandleFail("ShouldBeGreaterThan", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this float value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="comparer">The <i>float</i> to compare to.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBeGreaterThanOrEqualTo(this float source, float comparer)
        {
            return source.ShouldBeGreaterThanOrEqualTo(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this float value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="comparer">The <i>float</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBeGreaterThanOrEqualTo(this float source, float comparer, string message)
        {
            return source.ShouldBeGreaterThanOrEqualTo(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this float value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="comparer">The <i>float</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBeGreaterThanOrEqualTo(this float source, float comparer, string message, params object[] parameters)
        {
            if (source < comparer)
            {
                AssertHelper.HandleFail("ShouldBeGreaterThanOrEqualTo", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this float value is less than a certain value.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="comparer">The <i>float</i> to compare to.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBeLessThan(this float source, float comparer)
        {
            return source.ShouldBeLessThan(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this float value is less than a certain value.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="comparer">The <i>float</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBeLessThan(this float source, float comparer, string message)
        {
            return source.ShouldBeLessThan(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this float value is less than a certain value.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="comparer">The <i>float</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBeLessThan(this float source, float comparer, string message, params object[] parameters)
        {
            if (source >= comparer)
            {
                AssertHelper.HandleFail("ShouldBeLessThan", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this float value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="comparer">The <i>float</i> to compare to.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBeLessThanOrEqualTo(this float source, float comparer)
        {
            return source.ShouldBeLessThanOrEqualTo(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this float value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="comparer">The <i>float</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBeLessThanOrEqualTo(this float source, float comparer, string message)
        {
            return source.ShouldBeLessThanOrEqualTo(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this float value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="comparer">The <i>float</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBeLessThanOrEqualTo(this float source, float comparer, string message, params object[] parameters)
        {
            if (source > comparer)
            {
                AssertHelper.HandleFail("ShouldBeLessThanOrEqualTo", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this float value is between two values.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>float</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>float</i> to be less than or equal to.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        public static float ShouldBeBetween(this float source, float lowerInclusive, float upperInclusive)
        {
            return source.ShouldBeBetween(lowerInclusive, upperInclusive, String.Empty);
        }

        /// <summary>
        /// Asserts that this float value is between two values.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>float</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>float</i> to be less than or equal to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        public static float ShouldBeBetween(this float source, float lowerInclusive, float upperInclusive, string message)
        {
            return source.ShouldBeBetween(lowerInclusive, upperInclusive, message, null);
        }

        /// <summary>
        /// Asserts that this float value is between two values.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>float</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>float</i> to be less than or equal to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        public static float ShouldBeBetween(this float source, float lowerInclusive, float upperInclusive, string message, params object[] parameters)
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
        /// Asserts that this float value is between two values.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>float</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>float</i> to be greater than.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        public static float ShouldNotBeBetween(this float source, float lowerExclusive, float upperExclusive)
        {
            return source.ShouldNotBeBetween(lowerExclusive, upperExclusive, String.Empty);
        }

        /// <summary>
        /// Asserts that this float value is between two values.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>float</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>float</i> to be greater than.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        public static float ShouldNotBeBetween(this float source, float lowerExclusive, float upperExclusive, string message)
        {
            return source.ShouldNotBeBetween(lowerExclusive, upperExclusive, message, null);
        }

        /// <summary>
        /// Asserts that this float value is between two values.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>float</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>float</i> to be greater than.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        public static float ShouldNotBeBetween(this float source, float lowerExclusive, float upperExclusive, string message, params object[] parameters)
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
        /// Asserts that this float value is positive or zero.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBePositive(this float source)
        {
            return source.ShouldBePositive(String.Empty);
        }

        /// <summary>
        /// Asserts that this float value is positive or zero.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBePositive(this float source, string message)
        {
            return source.ShouldBePositive(message, null);
        }

        /// <summary>
        /// Asserts that this float value is positive or zero.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBePositive(this float source, string message, params object[] parameters)
        {
            if (source < 0)
            {
                AssertHelper.HandleFail("ShouldBePositive", 
                    message.AppendMessage(Messages.ActualString, source), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this float value is negative.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBeNegative(this float source)
        {
            return source.ShouldBeNegative(String.Empty);
        }

        /// <summary>
        /// Asserts that this float value is negative.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBeNegative(this float source, string message)
        {
            return source.ShouldBeNegative(message, null);
        }

        /// <summary>
        /// Asserts that this float value is negative.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBeNegative(this float source, string message, params object[] parameters)
        {
            if (source >= 0)
            {
                AssertHelper.HandleFail("ShouldBeNegative", 
                    message.AppendMessage(Messages.ActualString, source), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this float value is integral (does not have a fractional component).
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBeIntegral(this float source)
        {
            return source.ShouldBeIntegral(String.Empty);
        }

        /// <summary>
        /// Asserts that this float value is integral (does not have a fractional component).
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBeIntegral(this float source, string message)
        {
            return source.ShouldBeIntegral(message, null);
        }

        /// <summary>
        /// Asserts that this float value is integral (does not have a fractional component).
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBeIntegral(this float source, string message, params object[] parameters)
        {
            if (Math.Abs(source - Math.Truncate(source)) > Single.Epsilon)
            {
                AssertHelper.HandleFail("ShouldBeIntegral", 
                    message.AppendMessage(Messages.ActualString, source), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this float value is fractional.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBeFractional(this float source)
        {
            return source.ShouldBeFractional(String.Empty);
        }

        /// <summary>
        /// Asserts that this float value is fractional.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBeFractional(this float source, string message)
        {
            return source.ShouldBeFractional(message, null);
        }

        /// <summary>
        /// Asserts that this float value is fractional.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldBeFractional(this float source, string message, params object[] parameters)
        {
            if (Math.Abs(source - Math.Truncate(source)) < Single.Epsilon)
            {
                AssertHelper.HandleFail("ShouldBeFractional", 
                    message.AppendMessage(Messages.ActualString, source), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this float value rounds to a specified value.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="comparer">The <i>long</i> to compare to.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldRoundTo(this float source, long comparer)
        {
            return source.ShouldRoundTo(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this float value rounds to a specified value.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="comparer">The <i>long</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldRoundTo(this float source, long comparer, string message)
        {
            return source.ShouldRoundTo(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this float value rounds to a specified value.
        /// </summary>
        /// <param name="source">The float value to test.</param>
        /// <param name="comparer">The <i>long</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>float</i> that was tested.</returns>
        public static float ShouldRoundTo(this float source, long comparer, string message, params object[] parameters)
        {
            var rounded = Math.Round(source);
            if (Math.Abs(rounded - comparer) > Single.Epsilon)
            {
                AssertHelper.HandleFail("ShouldRoundTo", 
                    message.AppendMessage(Messages.ActualExpectedString, rounded, comparer), parameters);
            }

            return source;
        }
    }
}
