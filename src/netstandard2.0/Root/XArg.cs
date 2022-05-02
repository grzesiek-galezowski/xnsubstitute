using System;
using System.Collections;
using FluentAssertions;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using TddXt.XFluentAssert.Api;
using TddXt.XNSubstitute.ImplementationDetails;

namespace TddXt.XNSubstitute;

public static class XArg
{
  public static T IsLike<T>(T expected)
  {
    return Where<T>(actual => actual.Should().BeLike(expected));
  }

  public static T IsNotLike<T>(T expected)
  {
    return Where<T>(actual => actual.Should().NotBeLike(expected));
  }

  public static T EquivalentTo<T>(T expected)
  {
    return Where<T>(actual => actual.Should().BeEquivalentTo(expected));
  }

  private static void EnqueueMatcher<T>(IArgumentMatcher lambdaMatcher)
  {
    SubstitutionContext.Current.ThreadContext.EnqueueArgumentSpecification(
      new ArgumentSpecification(typeof(T), 
        lambdaMatcher));
  }

  public static TCollection SequenceEqualTo<TCollection>(TCollection expected) where TCollection : IEnumerable
  {
    return Where<TCollection>(actual => actual.Should().BeEquivalentTo(
      expected, 
      options => options.WithStrictOrdering()));
  }

  public static TCollection NotSequenceEqualTo<TCollection>(TCollection expected) where TCollection : IEnumerable
  {
    return Where<TCollection>(actual => actual.Should()
      .NotBeEquivalentTo(expected, options => options.WithStrictOrdering()));
  }

  internal static T Where<T>(params Action<T>[] assertions)
  {
    assertions
      .Should().NotBeEmpty("at least one condition should be specified");

    var lambdaMatcher = new LambdaArgumentMatcher<T>(assertions);
    EnqueueMatcher<T>(lambdaMatcher);
    return default;
  }

}