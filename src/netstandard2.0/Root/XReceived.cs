namespace TddXt.XNSubstitute.Root
{
  using System;

  using NSubstitute.Core;

  using ImplementationDetails;

  public class XReceived
  {
    public static void Only(Action action)
    {
      new SequenceExclusiveAssertion(
        new FilterAllowingPropertyGetters()
        ).Assert(SubstitutionContext.Current.RunQuery(action));
    }

    public static void Only(Action action, IQueryFilter methodFilter)
    {
      new SequenceExclusiveAssertion(
        Allow.AllOf(Allow.PropertyGetters(), methodFilter)
        ).Assert(SubstitutionContext.Current.RunQuery(action));
      //bug do not use obsolete API. Instead: https://github.com/nsubstitute/NSubstitute/blob/3446605c077438bef91c349a5249a12644ffca37/src/NSubstitute/Received.cs
    }
  }
}
