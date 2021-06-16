using System;
using System.Diagnostics;

namespace Qowaiv.DomainModel.TestTools
{
    /// <summary>Minimized assert helper class, to prevent dependencies on test frameworks.</summary>
    internal static class Assert
    {
        /// <summary>Asserts that the object is not null. Throws if not.</summary>
        /// <param name="obj">
        /// The object that should not be null.
        /// </param>
        /// <param name="message">
        /// The optional failure message.
        /// </param>
        [DebuggerStepThrough]
        public static void IsNotNull([ValidatedNotNull]object obj, string message = null)
        {
            if (obj is null)
            {
                Fail(message);
            }
        }

        /// <summary>Throws an <see cref="AssertionFailed"/>.</summary>
        /// <param name="message">
        /// The failure message.
        /// </param>
        [DebuggerStepThrough]
        public static void Fail(string message)
        {
            if (message is null)
            {
                throw new AssertionFailed();
            }
            else
            {
                throw new AssertionFailed(message);
            }
        }

        /// <summary>Marks the NotNull argument as being validated for not being null, to satisfy the static code analysis.</summary>
        /// <remarks>
        /// Notice that it does not matter what this attribute does, as long as
        /// it is named ValidatedNotNullAttribute.
        /// </remarks>
        [AttributeUsage(AttributeTargets.Parameter)]
        internal sealed class ValidatedNotNullAttribute : Attribute { }
    }
}
