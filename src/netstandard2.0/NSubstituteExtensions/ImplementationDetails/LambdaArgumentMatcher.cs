namespace TddXt.XFluentAssert.NSubstituteExtensions
{
  using System;

  using NSubstitute.Core;
  using NSubstitute.Core.Arguments;

  using TddXt.XFluentAssert.NSubstituteExtensions.ImplementationDetails;

  public class LambdaArgumentMatcher<T> : IArgumentMatcher, IDescribeNonMatches
  {
    private readonly Action<T>[] _assertionActions;

    public LambdaArgumentMatcher(Action<T>[] assertionActions)
    {
      _assertionActions = assertionActions;
    }

    public bool IsSatisfiedBy(object actual)
    {
      try
      {
        InMatcherAssertionsEngine.Execute(_assertionActions, (T) actual);
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public string DescribeFor(object actual)
    {
      try
      {
        InMatcherAssertionsEngine.Execute(_assertionActions, (T) actual);
        return string.Empty;
      }
      catch (Exception e)
      {
        return e.Message;
      }
    }
  }
}