using System;
using System.Collections.Generic;
using System.Linq;

namespace SeemplestBlocks.Testing.WinRtExtensions
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> does not contain any items.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldBeEmpty<T>(this IEnumerable<T> source)
        {
            return source.ShouldBeEmpty(String.Empty);
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> does not contain any items.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldBeEmpty<T>(this IEnumerable<T> source, string message)
        {
            return source.ShouldBeEmpty(message, null);
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> does not contain any items.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldBeEmpty<T>(this IEnumerable<T> source, string message, params object[] parameters)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            if (source.Any())
            {
                AssertHelper.HandleFail("ShouldBeEmpty", message, parameters);
            }

            // ReSharper disable once PossibleMultipleEnumeration
            return source;
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> contains items.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldNotBeEmpty<T>(this IEnumerable<T> source)
        {
            return source.ShouldNotBeEmpty(String.Empty);
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> contains items.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldNotBeEmpty<T>(this IEnumerable<T> source, string message)
        {
            return source.ShouldNotBeEmpty(message, null);
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> contains items.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldNotBeEmpty<T>(this IEnumerable<T> source, string message, params object[] parameters)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            if (!source.Any())
            {
                AssertHelper.HandleFail("ShouldNotBeEmpty", message, parameters);
            }

            // ReSharper disable once PossibleMultipleEnumeration
            return source;
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> contains a specific item.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <param name="item">The item to check for.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldContain<T>(this IEnumerable<T> source, T item)
        {
            return source.ShouldContain(item, String.Empty);
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> contains a specific item.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <param name="item">The item to check for.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldContain<T>(this IEnumerable<T> source, T item, string message)
        {
            return source.ShouldContain(item, String.Empty, null);
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> contains a specific item.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <param name="item">The item to check for.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldContain<T>(this IEnumerable<T> source, T item, string message, params object[] parameters)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            if (!source.Contains(item))
            {
                AssertHelper.HandleFail("ShouldContain", message, parameters);
            }

            // ReSharper disable once PossibleMultipleEnumeration
            return source;
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> does not contain a specific item.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <param name="item">The item to check for.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldNotContain<T>(this IEnumerable<T> source, T item)
        {
            return source.ShouldNotContain(item, String.Empty);
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> does not contain a specific item.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <param name="item">The item to check for.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldNotContain<T>(this IEnumerable<T> source, T item, string message)
        {
            return source.ShouldNotContain(item, String.Empty, null);
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> does not contain a specific item.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <param name="item">The item to check for.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldNotContain<T>(this IEnumerable<T> source, T item, string message, params object[] parameters)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            if (source.Contains(item))
            {
                AssertHelper.HandleFail("ShouldNotContain", message, parameters);
            }

            // ReSharper disable once PossibleMultipleEnumeration
            return source;
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> contains a specified number of items.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <param name="count">The expected count.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldHaveCountOf<T>(this IEnumerable<T> source, int count)
        {
            return source.ShouldHaveCountOf(count, String.Empty);
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> contains a specified number of items.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <param name="count">The expected count.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldHaveCountOf<T>(this IEnumerable<T> source, int count, string message)
        {
            return source.ShouldHaveCountOf(count, message, null);
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> contains a specified number of items.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <param name="count">The expected count.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldHaveCountOf<T>(this IEnumerable<T> source, int count, string message, params object[] parameters)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            if (source.Count() != count)
            {
                AssertHelper.HandleFail("ShouldHaveCountOf",
                    // ReSharper disable PossibleMultipleEnumeration
                    message.AppendMessage(Messages.ActualExpectedString, source != null 
                    ? source.Count().ToString() 
                    : Messages.NullString, count), parameters);
                   // ReSharper restore PossibleMultipleEnumeration
            }

            // ReSharper disable once PossibleMultipleEnumeration
            return source;
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> does not contain a specified number of items.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <param name="count">The count that enumerable should not have.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldNotHaveCountOf<T>(this IEnumerable<T> source, int count)
        {
            return source.ShouldNotHaveCountOf(count, String.Empty);
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> does not contain a specified number of items.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <param name="count">The count that enumerable should not have.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldNotHaveCountOf<T>(this IEnumerable<T> source, int count, string message)
        {
            return source.ShouldNotHaveCountOf(count, message, null);
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> does not contain a specified number of items.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <param name="count">The count that enumerable should not have.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldNotHaveCountOf<T>(this IEnumerable<T> source, int count, string message, params object[] parameters)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            if (source.Count() == count)
            {
                AssertHelper.HandleFail("ShouldNotHaveCountOf",
                    // ReSharper disable PossibleMultipleEnumeration
                    message.AppendMessage(Messages.ActualExpectedString, source != null 
                    ? source.Count().ToString() 
                    : Messages.NullString, count), parameters);
                    // ReSharper restore PossibleMultipleEnumeration
            }

            // ReSharper disable once PossibleMultipleEnumeration
            return source;
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> contains a minimum specified number of items.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <param name="count">The minimum expected count.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldHaveCountOfAtLeast<T>(this IEnumerable<T> source, int count)
        {
            return source.ShouldHaveCountOfAtLeast(count, String.Empty);
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> contains a minimum specified number of items.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <param name="count">The minimum expected count.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldHaveCountOfAtLeast<T>(this IEnumerable<T> source, int count, string message)
        {
            return source.ShouldHaveCountOfAtLeast(count, message, null);
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> contains a minimum specified number of items.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <param name="count">The minimum expected count.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldHaveCountOfAtLeast<T>(this IEnumerable<T> source, int count, string message, params object[] parameters)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            if (source.Count() < count)
            {
                AssertHelper.HandleFail("ShouldHaveCountOfAtLeast",
                    // ReSharper disable PossibleMultipleEnumeration
                    message.AppendMessage(Messages.ActualExpectedString, source != null 
                    ? source.Count().ToString() 
                    : Messages.NullString, count), parameters);
                    // ReSharper restore PossibleMultipleEnumeration
            }

            // ReSharper disable once PossibleMultipleEnumeration
            return source;
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> contains a maximum specified number of items.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <param name="count">The maximum expected count.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldHaveCountOfAtMost<T>(this IEnumerable<T> source, int count)
        {
            return source.ShouldHaveCountOfAtMost(count, String.Empty);
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> contains a maximum specified number of items.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <param name="count">The maximum expected count.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldHaveCountOfAtMost<T>(this IEnumerable<T> source, int count, string message)
        {
            return source.ShouldHaveCountOfAtMost(count, message, null);
        }

        /// <summary>
        /// Asserts that this <see cref="IEnumerable{T}"/> contains a maximum specified number of items.
        /// </summary>
        /// <typeparam name="T">The type that the enumerable contains.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to test.</param>
        /// <param name="count">The maximum expected count.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <see cref="IEnumerable{T}"/> that was tested.</returns>
        public static IEnumerable<T> ShouldHaveCountOfAtMost<T>(this IEnumerable<T> source, int count, string message, params object[] parameters)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            if (source.Count() > count)
            {
                AssertHelper.HandleFail("ShouldHaveCountOfAtLeast",
                    // ReSharper disable PossibleMultipleEnumeration
                    message.AppendMessage(Messages.ActualExpectedString, source != null 
                    ? source.Count().ToString() 
                    : Messages.NullString, count), parameters);
                    // ReSharper restore PossibleMultipleEnumeration
            }

            // ReSharper disable once PossibleMultipleEnumeration
            return source;
        }
    }
}
