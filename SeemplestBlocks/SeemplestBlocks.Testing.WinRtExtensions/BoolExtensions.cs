using System;

namespace SeemplestBlocks.Testing.WinRtExtensions
{
    /// <summary>
    /// Provides a set of extension methods to facilitate readable testing assertions.
    /// </summary>
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Asserts that this boolean value is false.
        /// </summary>
        /// <param name="source">The <i>boolean</i> value to test.</param>
        /// <returns>The original <i>boolean</i> that was tested.</returns>
        public static bool ShouldBeFalse(this bool source)
        {
            return source.ShouldBeFalse(String.Empty);
        }

        /// <summary>
        /// Asserts that this boolean value is false.
        /// </summary>
        /// <param name="source">The <i>boolean</i> value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>boolean</i> that was tested.</returns>
        public static bool ShouldBeFalse(this bool source, string message)
        {
            return source.ShouldBeFalse(message, null);
        }

        /// <summary>
        /// Asserts that this boolean value is false.
        /// </summary>
        /// <param name="source">The <i>boolean</i> value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>boolean</i> that was tested.</returns>
        public static bool ShouldBeFalse(this bool source, string message, params object[] parameters)
        {
            if (source)
                AssertHelper.HandleFail("ShouldBeFalse", message, parameters);

            return source;
        }

        /// <summary>
        /// Asserts that this nullable boolean value is false.
        /// </summary>
        /// <param name="source">The nullable boolean value to test.</param>
        /// <returns>The original nullable boolean that was tested.</returns>
        public static bool? ShouldBeFalse(this bool? source)
        {
            return source.ShouldBeFalse(String.Empty);
        }

        /// <summary>
        /// Asserts that this nullable boolean value is false.
        /// </summary>
        /// <param name="source">The nullable boolean value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original nullable boolean that was tested.</returns>
        public static bool? ShouldBeFalse(this bool? source, string message)
        {
            return source.ShouldBeFalse(message, null);
        }

        /// <summary>
        /// Asserts that this nullable boolean value is false.
        /// </summary>
        /// <param name="source">The nullable boolean value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original nullable boolean that was tested.</returns>
        public static bool? ShouldBeFalse(this bool? source, string message, params object[] parameters)
        {
            if (!source.HasValue || source.Value)
                AssertHelper.HandleFail("ShouldBeFalse", message, parameters);

            return source;
        }

        /// <summary>
        /// Asserts that this boolean value is true.
        /// </summary>
        /// <param name="source">The <i>boolean</i> value to test.</param>
        /// <returns>The original <i>boolean</i> that was tested.</returns>
        public static bool ShouldBeTrue(this bool source)
        {
            return source.ShouldBeTrue(String.Empty);
        }

        /// <summary>
        /// Asserts that this boolean value is true.
        /// </summary>
        /// <param name="source">The <i>boolean</i> value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <i>boolean</i> that was tested.</returns>
        public static bool ShouldBeTrue(this bool source, string message)
        {
            return source.ShouldBeTrue(message, null);
        }

        /// <summary>
        /// Asserts that this boolean value is true.
        /// </summary>
        /// <param name="source">The <i>boolean</i> value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <i>boolean</i> that was tested.</returns>
        public static bool ShouldBeTrue(this bool source, string message, params object[] parameters)
        {
            if (!source)
                AssertHelper.HandleFail("ShouldBeTrue", message, parameters);

            return source;
        }

        /// <summary>
        /// Asserts that this nullable boolean value is true.
        /// </summary>
        /// <param name="source">The nullable boolean value to test.</param>
        /// <returns>The original nullable boolean that was tested.</returns>
        public static bool? ShouldBeTrue(this bool? source)
        {
            return source.ShouldBeTrue(String.Empty);
        }

        /// <summary>
        /// Asserts that this nullable boolean value is true.
        /// </summary>
        /// <param name="source">The nullable boolean value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original nullable boolean that was tested.</returns>
        public static bool? ShouldBeTrue(this bool? source, string message)
        {
            return source.ShouldBeTrue(message, null);
        }

        /// <summary>
        /// Asserts that this nullable boolean value is true.
        /// </summary>
        /// <param name="source">The nullable boolean value to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original nullable boolean that was tested.</returns>
        public static bool? ShouldBeTrue(this bool? source, string message, params object[] parameters)
        {
            if (!source.HasValue || !source.Value)
                AssertHelper.HandleFail("ShouldBeTrue", message, parameters);

            return source;
        }
    }
}
