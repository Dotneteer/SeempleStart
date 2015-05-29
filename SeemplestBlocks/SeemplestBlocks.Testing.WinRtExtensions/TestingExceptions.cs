using System;
using System.Diagnostics.CodeAnalysis;

namespace SeemplestBlocks.Testing.WinRtExtensions
{
    /// <summary>
    /// Provides a set of static methods for testing.
    /// </summary>
    public static partial class Testing
    {
        /// <summary>
        /// Asserts that the method should throw an exception.
        /// </summary>
        /// <typeparam name="TExceptionType">The type of the expected exception.</typeparam>
        /// <param name="action">An <see cref="Action"/> to perform exception testing on.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <i>action</i> parameter is null.</exception>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static TExceptionType ShouldThrowException<TExceptionType>(Action action) where TExceptionType : Exception
        {
            return ShouldThrowException<TExceptionType>(action, String.Empty);
        }

        /// <summary>
        /// Asserts that the method should throw an exception.
        /// </summary>
        /// <typeparam name="TExceptionType">The type of the expected exception.</typeparam>
        /// <param name="action">An <see cref="Action"/> to perform exception testing on.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <i>action</i> parameter is null.</exception>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static TExceptionType ShouldThrowException<TExceptionType>(Action action, string message) where TExceptionType : Exception
        {
            return ShouldThrowException<TExceptionType>(action, message, null);
        }

        /// <summary>
        /// Asserts that the method should throw an exception.
        /// </summary>
        /// <typeparam name="TExceptionType">The type of the expected exception.</typeparam>
        /// <param name="action">An <see cref="Action"/> to perform exception testing on.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        /// <param name="parameters">An array of parameters to use with a formatted message.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <i>action</i> parameter is null.</exception>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static TExceptionType ShouldThrowException<TExceptionType>(Action action, string message, params object[] parameters) where TExceptionType : Exception
        {
            if (action == null)
                throw new ArgumentNullException("action");

            try
            {
                action();
            }
            catch (TExceptionType ex)
            {
                if (typeof(TExceptionType) == ex.GetType())
                    return ex;
                else
                    AssertHelper.HandleFail("ShouldThrowException",
                        message.AppendMessage(Messages.ExceptionActualExpectedString, ex.GetType().ToString(), (typeof(TExceptionType)).ToString()),
                        parameters);
            }
            catch (Exception ex)
            {
                AssertHelper.HandleFail("ShouldThrowException",
                    message.AppendMessage(Messages.ExceptionActualExpectedString, ex.GetType().ToString(), (typeof(TExceptionType)).ToString()),
                    parameters);
            }

            AssertHelper.HandleFail("ShouldThrowException",
                    message.AppendMessage(Messages.ExceptionNoneThrownString, (typeof(TExceptionType)).ToString()), parameters);
            return null;
        }
    }
}
