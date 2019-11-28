using System;
using System.Diagnostics;

namespace Qowaiv.DomainModel.TestTools
{
    /// <summary>Minimized assert helper class, to prevent dependencies on test frameworks.</summary>
    internal static class Assert
    {
        [DebuggerStepThrough]
        public static void IsNotNull([ValidatedNotNull]object obj, string message = null)
        {
            if (obj is null)
            {
                Fail(message);
            }
        }

        [DebuggerStepThrough]
        public static void Fail(string message)
        {
            throw new AssertException(message);
        }

        /// <summary>Marks the NotNull argument as being validated for not being null, to satisfy the static code analysis.</summary>
        /// <remarks>
        /// Notice that it does not matter what this attribute does, as long as
        /// it is named ValidatedNotNullAttribute.
        /// </remarks>
        [AttributeUsage(AttributeTargets.Parameter)]
        sealed class ValidatedNotNullAttribute : Attribute { }
    }
}
