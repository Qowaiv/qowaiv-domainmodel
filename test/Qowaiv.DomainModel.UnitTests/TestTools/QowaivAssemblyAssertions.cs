﻿using FluentAssertions.Execution;
using FluentAssertions.Reflection;
using System.Reflection;

namespace FluentAssertions;

/// <summary>Extensions on <see cref="AssemblyAssertions"/>.</summary>
internal static class QowaivAssemblyAssertions
{
    /// <summary>Asserts the <see cref="Assembly"/> to have a specific public key.</summary>
    public static AndConstraint<AssemblyAssertions> HavePublicKey(this AssemblyAssertions assertions, string publicKey, string because = "", params object[] becauseArgs)
    {
        Guard.NotNull(assertions, nameof(assertions));

        var bytes = assertions.Subject.GetName().GetPublicKey() ?? Array.Empty<byte>();
        var assemblyKey = BitConverter.ToString(bytes).Replace("-", "");

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(assemblyKey == publicKey)
            .FailWith(
            $"Expected '{assertions.Subject}' to have public key: {publicKey},{Environment.NewLine}" +
            $"but got: {assemblyKey} instead.");

        return new AndConstraint<AssemblyAssertions>(assertions);
    }
}
