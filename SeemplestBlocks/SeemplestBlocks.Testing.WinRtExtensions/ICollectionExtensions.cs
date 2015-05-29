using System;
using System.Collections.Generic;
using System.Linq;

namespace SeemplestBlocks.Testing.WinRtExtensions
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Asserts that this <see cref="ICollection{T}"/> does not contain any items.
        /// </summary>
        /// <typeparam name="T">The type that the collection contains.</typeparam>
        /// <param name="source">The <see cref="ICollection{T}"/> to test.</param>
        /// <returns>The original <see cref="ICollection{T}"/> that was tested.</returns>
        public static ICollection<T> ShouldBeEmpty<T>(this ICollection<T> source)
        {
            return source.ShouldBeEmpty(String.Empty);
        }

        /// <summary>
        /// Asserts that this <see cref="ICollection{T}"/> does not contain any items.
        /// </summary>
        /// <typeparam name="T">The type that the collection contains.</typeparam>
        /// <param name="source">The <see cref="ICollection{T}"/> to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <see cref="ICollection{T}"/> that was tested.</returns>
        public static ICollection<T> ShouldBeEmpty<T>(this ICollection<T> source, string message)
        {
            return source.ShouldBeEmpty(message, null);
        }

        /// <summary>
        /// Asserts that this <see cref="ICollection{T}"/> does not contain any items.
        /// </summary>
        /// <typeparam name="T">The type that the collection contains.</typeparam>
        /// <param name="source">The <see cref="ICollection{T}"/> to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <see cref="ICollection{T}"/> that was tested.</returns>
        public static ICollection<T> ShouldBeEmpty<T>(this ICollection<T> source, string message, params object[] parameters)
        {
            if (source.Any())
            {
                AssertHelper.HandleFail("ShouldBeEmpty", message, parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this <see cref="ICollection{T}"/> contains items.
        /// </summary>
        /// <typeparam name="T">The type that the collection contains.</typeparam>
        /// <param name="source">The <see cref="ICollection{T}"/> to test.</param>
        /// <returns>The original <see cref="ICollection{T}"/> that was tested.</returns>
        public static ICollection<T> ShouldNotBeEmpty<T>(this ICollection<T> source)
        {
            return source.ShouldNotBeEmpty(String.Empty);
        }

        /// <summary>
        /// Asserts that this <see cref="ICollection{T}"/> contains items.
        /// </summary>
        /// <typeparam name="T">The type that the collection contains.</typeparam>
        /// <param name="source">The <see cref="ICollection{T}"/> to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <see cref="ICollection{T}"/> that was tested.</returns>
        public static ICollection<T> ShouldNotBeEmpty<T>(this ICollection<T> source, string message)
        {
            return source.ShouldNotBeEmpty(message, null);
        }

        /// <summary>
        /// Asserts that this <see cref="ICollection{T}"/> contains items.
        /// </summary>
        /// <typeparam name="T">The type that the collection contains.</typeparam>
        /// <param name="source">The <see cref="ICollection{T}"/> to test.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <see cref="ICollection{T}"/> that was tested.</returns>
        public static ICollection<T> ShouldNotBeEmpty<T>(this ICollection<T> source, string message, params object[] parameters)
        {
            if (!source.Any())
            {
                AssertHelper.HandleFail("ShouldNotBeEmpty", message, parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this <see cref="ICollection{T}"/> contains a specific item.
        /// </summary>
        /// <typeparam name="T">The type that the collection contains.</typeparam>
        /// <param name="source">The <see cref="ICollection{T}"/> to test.</param>
        /// <param name="item">The item to check for.</param>
        /// <returns>The original <see cref="ICollection{T}"/> that was tested.</returns>
        public static ICollection<T> ShouldContain<T>(this ICollection<T> source, T item)
        {
            return source.ShouldContain(item, String.Empty);
        }

        /// <summary>
        /// Asserts that this <see cref="ICollection{T}"/> contains a specific item.
        /// </summary>
        /// <typeparam name="T">The type that the collection contains.</typeparam>
        /// <param name="source">The <see cref="ICollection{T}"/> to test.</param>
        /// <param name="item">The item to check for.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <see cref="ICollection{T}"/> that was tested.</returns>
        public static ICollection<T> ShouldContain<T>(this ICollection<T> source, T item, string message)
        {
            return source.ShouldContain(item, String.Empty, null);
        }

        /// <summary>
        /// Asserts that this <see cref="ICollection{T}"/> contains a specific item.
        /// </summary>
        /// <typeparam name="T">The type that the collection contains.</typeparam>
        /// <param name="source">The <see cref="ICollection{T}"/> to test.</param>
        /// <param name="item">The item to check for.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <see cref="ICollection{T}"/> that was tested.</returns>
        public static ICollection<T> ShouldContain<T>(this ICollection<T> source, T item, string message, params object[] parameters)
        {
            if (!source.Contains(item))
            {
                AssertHelper.HandleFail("ShouldContain", message, parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this <see cref="ICollection{T}"/> does not contain a specific item.
        /// </summary>
        /// <typeparam name="T">The type that the collection contains.</typeparam>
        /// <param name="source">The <see cref="ICollection{T}"/> to test.</param>
        /// <param name="item">The item to check for.</param>
        /// <returns>The original <see cref="ICollection{T}"/> that was tested.</returns>
        public static ICollection<T> ShouldNotContain<T>(this ICollection<T> source, T item)
        {
            return source.ShouldNotContain(item, String.Empty);
        }

        /// <summary>
        /// Asserts that this <see cref="ICollection{T}"/> does not contain a specific item.
        /// </summary>
        /// <typeparam name="T">The type that the collection contains.</typeparam>
        /// <param name="source">The <see cref="ICollection{T}"/> to test.</param>
        /// <param name="item">The item to check for.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <see cref="ICollection{T}"/> that was tested.</returns>
        public static ICollection<T> ShouldNotContain<T>(this ICollection<T> source, T item, string message)
        {
            return source.ShouldNotContain(item, String.Empty, null);
        }

        /// <summary>
        /// Asserts that this <see cref="ICollection{T}"/> does not contain a specific item.
        /// </summary>
        /// <typeparam name="T">The type that the collection contains.</typeparam>
        /// <param name="source">The <see cref="ICollection{T}"/> to test.</param>
        /// <param name="item">The item to check for.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <see cref="ICollection{T}"/> that was tested.</returns>
        public static ICollection<T> ShouldNotContain<T>(this ICollection<T> source, T item, string message, params object[] parameters)
        {
            if (source.Contains(item))
            {
                AssertHelper.HandleFail("ShouldNotContain", message, parameters);
            }

            return source;
        }

        /// <summary>
        /// Asserts that this <see cref="ICollection{T}"/> contains a specified number of items..
        /// </summary>
        /// <typeparam name="T">The type that the collection contains.</typeparam>
        /// <param name="source">The <see cref="ICollection{T}"/> to test.</param>
        /// <param name="count">The expected count.</param>
        /// <returns>The original <see cref="ICollection{T}"/> that was tested.</returns>
        public static ICollection<T> ShouldHaveCountOf<T>(this ICollection<T> source, int count)
        {
            return source.ShouldHaveCountOf(count, String.Empty);
        }

        /// <summary>
        /// Asserts that this <see cref="ICollection{T}"/> contains a specified number of items..
        /// </summary>
        /// <typeparam name="T">The type that the collection contains.</typeparam>
        /// <param name="source">The <see cref="ICollection{T}"/> to test.</param>
        /// <param name="count">The expected count.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <returns>The original <see cref="ICollection{T}"/> that was tested.</returns>
        public static ICollection<T> ShouldHaveCountOf<T>(this ICollection<T> source, int count, string message)
        {
            return source.ShouldHaveCountOf(count, message, null);
        }

        /// <summary>
        /// Asserts that this <see cref="ICollection{T}"/> contains a specified number of items..
        /// </summary>
        /// <typeparam name="T">The type that the collection contains.</typeparam>
        /// <param name="source">The <see cref="ICollection{T}"/> to test.</param>
        /// <param name="count">The expected count.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <returns>The original <see cref="ICollection{T}"/> that was tested.</returns>
        public static ICollection<T> ShouldHaveCountOf<T>(this ICollection<T> source, int count, string message, params object[] parameters)
        {
            if (source.Count != count)
            {
                AssertHelper.HandleFail("ShouldHaveCountOf", 
                    message.AppendMessage(Messages.ActualExpectedString, source.Count, count), parameters);
            }

            return source;
        }
    }
}
