using System;

namespace SeemplestBlocks.Testing.WinRtExtensions
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Asserts that this sbyte value is greater than a certain value.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <param name="comparer">The <i>sbyte</i> to compare to.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        [CLSCompliant(false)]
        public static sbyte ShouldBeGreaterThan(this sbyte source, sbyte comparer)
        {
            return source.ShouldBeGreaterThan(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this sbyte value is greater than a certain value.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <param name="comparer">The <i>sbyte</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        [CLSCompliant(false)]
        public static sbyte ShouldBeGreaterThan(this sbyte source, sbyte comparer, string message)
        {
            return source.ShouldBeGreaterThan(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this sbyte value is greater than a certain value.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <param name="comparer">The <i>sbyte</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        [CLSCompliant(false)]
        public static sbyte ShouldBeGreaterThan(this sbyte source, sbyte comparer, string message, params object[] parameters)
        {
            if (source <= comparer)
            {
                AssertHelper.HandleFail("ShouldBeGreaterThan", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this sbyte value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <param name="comparer">The <i>sbyte</i> to compare to.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        [CLSCompliant(false)]
        public static sbyte ShouldBeGreaterThanOrEqualTo(this sbyte source, sbyte comparer)
        {
            return source.ShouldBeGreaterThanOrEqualTo(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this sbyte value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <param name="comparer">The <i>sbyte</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        [CLSCompliant(false)]
        public static sbyte ShouldBeGreaterThanOrEqualTo(this sbyte source, sbyte comparer, string message)
        {
            return source.ShouldBeGreaterThanOrEqualTo(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this sbyte value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <param name="comparer">The <i>sbyte</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        [CLSCompliant(false)]
        public static sbyte ShouldBeGreaterThanOrEqualTo(this sbyte source, sbyte comparer, string message, params object[] parameters)
        {
            if (source < comparer)
            {
                AssertHelper.HandleFail("ShouldBeGreaterThanOrEqualTo", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this sbyte value is less than a certain value.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <param name="comparer">The <i>sbyte</i> to compare to.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        [CLSCompliant(false)]
        public static sbyte ShouldBeLessThan(this sbyte source, sbyte comparer)
        {
            return source.ShouldBeLessThan(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this sbyte value is less than a certain value.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <param name="comparer">The <i>sbyte</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        [CLSCompliant(false)]
        public static sbyte ShouldBeLessThan(this sbyte source, sbyte comparer, string message)
        {
            return source.ShouldBeLessThan(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this sbyte value is less than a certain value.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <param name="comparer">The <i>sbyte</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        [CLSCompliant(false)]
        public static sbyte ShouldBeLessThan(this sbyte source, sbyte comparer, string message, params object[] parameters)
        {
            if (source >= comparer)
            {
                AssertHelper.HandleFail("ShouldBeLessThan", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this sbyte value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <param name="comparer">The <i>sbyte</i> to compare to.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        [CLSCompliant(false)]
        public static sbyte ShouldBeLessThanOrEqualTo(this sbyte source, sbyte comparer)
        {
            return source.ShouldBeLessThanOrEqualTo(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this sbyte value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <param name="comparer">The <i>sbyte</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        [CLSCompliant(false)]
        public static sbyte ShouldBeLessThanOrEqualTo(this sbyte source, sbyte comparer, string message)
        {
            return source.ShouldBeLessThanOrEqualTo(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this sbyte value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <param name="comparer">The <i>sbyte</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        [CLSCompliant(false)]
        public static sbyte ShouldBeLessThanOrEqualTo(this sbyte source, sbyte comparer, string message, params object[] parameters)
        {
            if (source > comparer)
            {
                AssertHelper.HandleFail("ShouldBeLessThanOrEqualTo", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this sbyte value is between two values.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>sbyte</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>sbyte</i> to be less than or equal to.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        [CLSCompliant(false)]
        public static sbyte ShouldBeBetween(this sbyte source, sbyte lowerInclusive, sbyte upperInclusive)
        {
            return source.ShouldBeBetween(lowerInclusive, upperInclusive, String.Empty);
        }

        /// <summary>
        /// Asserts that this sbyte value is between two values.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>sbyte</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>sbyte</i> to be less than or equal to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        [CLSCompliant(false)]
        public static sbyte ShouldBeBetween(this sbyte source, sbyte lowerInclusive, sbyte upperInclusive, string message)
        {
            return source.ShouldBeBetween(lowerInclusive, upperInclusive, message, null);
        }

        /// <summary>
        /// Asserts that this sbyte value is between two values.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>sbyte</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>sbyte</i> to be less than or equal to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        [CLSCompliant(false)]
        public static sbyte ShouldBeBetween(this sbyte source, sbyte lowerInclusive, sbyte upperInclusive, string message, params object[] parameters)
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
        /// Asserts that this sbyte value is between two values.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>sbyte</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>sbyte</i> to be greater than.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        [CLSCompliant(false)]
        public static sbyte ShouldNotBeBetween(this sbyte source, sbyte lowerExclusive, sbyte upperExclusive)
        {
            return source.ShouldNotBeBetween(lowerExclusive, upperExclusive, String.Empty);
        }

        /// <summary>
        /// Asserts that this sbyte value is between two values.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>sbyte</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>sbyte</i> to be greater than.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        [CLSCompliant(false)]
        public static sbyte ShouldNotBeBetween(this sbyte source, sbyte lowerExclusive, sbyte upperExclusive, string message)
        {
            return source.ShouldNotBeBetween(lowerExclusive, upperExclusive, message, null);
        }

        /// <summary>
        /// Asserts that this sbyte value is between two values.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>sbyte</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>sbyte</i> to be greater than.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        [CLSCompliant(false)]
        public static sbyte ShouldNotBeBetween(this sbyte source, sbyte lowerExclusive, sbyte upperExclusive, string message, params object[] parameters)
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
        /// Asserts that this sbyte value is positive or zero.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        [CLSCompliant(false)]
        public static sbyte ShouldBePositive(this sbyte source)
        {
            return source.ShouldBePositive(String.Empty);
        }

        /// <summary>
        /// Asserts that this sbyte value is positive or zero.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        [CLSCompliant(false)]
        public static sbyte ShouldBePositive(this sbyte source, string message)
        {
            return source.ShouldBePositive(message, null);
        }

        /// <summary>
        /// Asserts that this sbyte value is positive or zero.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        [CLSCompliant(false)]
        public static sbyte ShouldBePositive(this sbyte source, string message, params object[] parameters)
        {
            if (source < 0)
            {
                AssertHelper.HandleFail("ShouldBePositive", 
                    message.AppendMessage(Messages.ActualString, source), parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this sbyte value is negative.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        [CLSCompliant(false)]
        public static sbyte ShouldBeNegative(this sbyte source)
        {
            return source.ShouldBeNegative(String.Empty);
        }

        /// <summary>
        /// Asserts that this sbyte value is negative.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        [CLSCompliant(false)]
        public static sbyte ShouldBeNegative(this sbyte source, string message)
        {
            return source.ShouldBeNegative(message, null);
        }

        /// <summary>
        /// Asserts that this sbyte value is negative.
        /// </summary>
        /// <param name="source">The sbyte value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>sbyte</i> that was tested.</returns>
        [CLSCompliant(false)]
        public static sbyte ShouldBeNegative(this sbyte source, string message, params object[] parameters)
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
