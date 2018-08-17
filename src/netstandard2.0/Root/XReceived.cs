namespace TddXt.XNSubstitute.Root
{
  using System;

  using NSubstitute.Core;

  using TddXt.XNSubstitute.Root.ImplementationDetails;

  public class XReceived
  {
    public static void Only(Action action)
    {
      new SequenceExclusiveAssertion().Assert(SubstitutionContext.Current.RunQuery(action));
    }
  }
}
