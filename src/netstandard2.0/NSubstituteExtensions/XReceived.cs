namespace TddXt.XFluentAssert.NSubstituteExtensions
{
  using System;

  using NSubstitute.Core;

  public class XReceived
  {
    public static void Only(Action action)
    {
      new SequenceExclusiveAssertion().Assert(SubstitutionContext.Current.RunQuery(action));
    }
  }
}
