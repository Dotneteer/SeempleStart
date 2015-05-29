using System;

namespace SeemplestBlocks.Testing.WinRtExtensions
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Asserts that this byte value is greater than a certain value.
        /// </summary>
        /// <param name="source">The byte value to test.</param>
        /// <param name="comparer">The <i>byte</i> to compare to.</param>
        /// <returns>The original <i>byte</i> that was tested.</returns>
        public static byte ShouldBeGreaterThan(this byte source, byte comparer)
        {
            return source.ShouldBeGreaterThan(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this byte value is greater than a certain value.
        /// </summary>
        /// <param name="source">The byte value to test.</param>
        /// <param name="comparer">The <i>byte</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>byte</i> that was tested.</returns>
        public static byte ShouldBeGreaterThan(this byte source, byte comparer, string message)
        {
            return source.ShouldBeGreaterThan(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this byte value is greater than a certain value.
        /// </summary>
        /// <param name="source">The byte value to test.</param>
        /// <param name="comparer">The <i>byte</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>byte</i> that was tested.</returns>
        public static byte ShouldBeGreaterThan(this byte source, byte comparer, string message, params object[] parameters)
        {
            if (source <= comparer)
            {
                AssertHelper.HandleFail("ShouldBeGreaterThan", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }
            return source;
        }

        /// <summary>
        /// Asserts that this byte value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The byte value to test.</param>
        /// <param name="comparer">The <i>byte</i> to compare to.</param>
        /// <returns>The original <i>byte</i> that was tested.</returns>
        public static byte ShouldBeGreaterThanOrEqualTo(this byte source, byte comparer)
        {
            return source.ShouldBeGreaterThanOrEqualTo(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this byte value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The byte value to test.</param>
        /// <param name="comparer">The <i>byte</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>byte</i> that was tested.</returns>
        public static byte ShouldBeGreaterThanOrEqualTo(this byte source, byte comparer, string message)
        {
            return source.ShouldBeGreaterThanOrEqualTo(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this byte value is greater than or equal to a certain value.
        /// </summary>
        /// <param name="source">The byte value to test.</param>
        /// <param name="comparer">The <i>byte</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>byte</i> that was tested.</returns>
        public static byte ShouldBeGreaterThanOrEqualTo(this byte source, byte comparer, string message, params object[] parameters)
        {
            if (source < comparer)
            {
                AssertHelper.HandleFail("ShouldBeGreaterThanOrEqualTo", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }
            return source;
        }

        /// <summary>
        /// Asserts that this byte value is less than a certain value.
        /// </summary>
        /// <param name="source">The byte value to test.</param>
        /// <param name="comparer">The <i>byte</i> to compare to.</param>
        /// <returns>The original <i>byte</i> that was tested.</returns>
        public static byte ShouldBeLessThan(this byte source, byte comparer)
        {
            return source.ShouldBeLessThan(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this byte value is less than a certain value.
        /// </summary>
        /// <param name="source">The byte value to test.</param>
        /// <param name="comparer">The <i>byte</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>byte</i> that was tested.</returns>
        public static byte ShouldBeLessThan(this byte source, byte comparer, string message)
        {
            return source.ShouldBeLessThan(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this byte value is less than a certain value.
        /// </summary>
        /// <param name="source">The byte value to test.</param>
        /// <param name="comparer">The <i>byte</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>byte</i> that was tested.</returns>
        public static byte ShouldBeLessThan(this byte source, byte comparer, string message, params object[] parameters)
        {
            if (source >= comparer)
            {
                AssertHelper.HandleFail("ShouldBeLessThan", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }
            return source;
        }

        /// <summary>
        /// Asserts that this byte value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The byte value to test.</param>
        /// <param name="comparer">The <i>byte</i> to compare to.</param>
        /// <returns>The original <i>byte</i> that was tested.</returns>
        public static byte ShouldBeLessThanOrEqualTo(this byte source, byte comparer)
        {
            return source.ShouldBeLessThanOrEqualTo(comparer, String.Empty);
        }

        /// <summary>
        /// Asserts that this byte value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The byte value to test.</param>
        /// <param name="comparer">The <i>byte</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>byte</i> that was tested.</returns>
        public static byte ShouldBeLessThanOrEqualTo(this byte source, byte comparer, string message)
        {
            return source.ShouldBeLessThanOrEqualTo(comparer, message, null);
        }

        /// <summary>
        /// Asserts that this byte value is less than or equal to a certain value.
        /// </summary>
        /// <param name="source">The byte value to test.</param>
        /// <param name="comparer">The <i>byte</i> to compare to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>byte</i> that was tested.</returns>
        public static byte ShouldBeLessThanOrEqualTo(this byte source, byte comparer, string message, params object[] parameters)
        {
            if (source > comparer)
            {
                AssertHelper.HandleFail("ShouldBeLessThanOrEqualTo", 
                    message.AppendMessage(Messages.ActualExpectedString, source, comparer), parameters);
            }
            return source;
        }

        /// <summary>
        /// Asserts that this byte value is between two values.
        /// </summary>
        /// <param name="source">The byte value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>byte</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>byte</i> to be less than or equal to.</param>
        /// <returns>The original <i>byte</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        public static byte ShouldBeBetween(this byte source, byte lowerInclusive, byte upperInclusive)
        {
            return source.ShouldBeBetween(lowerInclusive, upperInclusive, String.Empty);
        }

        /// <summary>
        /// Asserts that this byte value is between two values.
        /// </summary>
        /// <param name="source">The byte value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>byte</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>byte</i> to be less than or equal to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>byte</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        public static byte ShouldBeBetween(this byte source, byte lowerInclusive, byte upperInclusive, string message)
        {
            return source.ShouldBeBetween(lowerInclusive, upperInclusive, message, null);
        }

        /// <summary>
        /// Asserts that this byte value is between two values.
        /// </summary>
        /// <param name="source">The byte value to test.</param>
        /// <param name="lowerInclusive">The lower inclusive <i>byte</i> to be greater than or equal to.</param>
        /// <param name="upperInclusive">The upper inclusive <i>byte</i> to be less than or equal to.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>byte</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerInclusive argument is greater than or equal to the upperInclusive argument.</exception>
        public static byte ShouldBeBetween(this byte source, byte lowerInclusive, byte upperInclusive, string message, params object[] parameters)
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
        /// Asserts that this byte value is between two values.
        /// </summary>
        /// <param name="source">The byte value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>byte</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>byte</i> to be greater than.</param>
        /// <returns>The original <i>byte</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        public static byte ShouldNotBeBetween(this byte source, byte lowerExclusive, byte upperExclusive)
        {
            return source.ShouldNotBeBetween(lowerExclusive, upperExclusive, String.Empty);
        }

        /// <summary>
        /// Asserts that this byte value is between two values.
        /// </summary>
        /// <param name="source">The byte value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>byte</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>byte</i> to be greater than.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>byte</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        public static byte ShouldNotBeBetween(this byte source, byte lowerExclusive, byte upperExclusive, string message)
        {
            return source.ShouldNotBeBetween(lowerExclusive, upperExclusive, message, null);
        }

        /// <summary>
        /// Asserts that this byte value is between two values.
        /// </summary>
        /// <param name="source">The byte value to test.</param>
        /// <param name="lowerExclusive">The lower inclusive <i>byte</i> to be less than.</param>
        /// <param name="upperExclusive">The upper inclusive <i>byte</i> to be greater than.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>byte</i> that was tested.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the lowerExclusive argument is greater than or equal to the upperExclusive argument.</exception>
        public static byte ShouldNotBeBetween(this byte source, byte lowerExclusive, byte upperExclusive, string message, params object[] parameters)
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
    }
}
