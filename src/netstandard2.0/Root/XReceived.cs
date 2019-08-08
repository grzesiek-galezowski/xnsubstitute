namespace TddXt.XNSubstitute.Root
{
  using System;

  using NSubstitute.Core;

  using ImplementationDetails;

  public class XReceived
  {
    public static void Only(Action action)
    {
      var query = new Query(SubstitutionContext.Current.CallSpecificationFactory);
      SubstitutionContext.Current.ThreadContext.RunInQueryContext(action, query);

      new SequenceExclusiveAssertion(
        new FilterAllowingPropertyGetters()
        ).Assert(query.Result());
    }

    public static void Only(Action action, IQueryFilter methodFilter)
    {
      var query = new Query(SubstitutionContext.Current.CallSpecificationFactory);
      SubstitutionContext.Current.ThreadContext.RunInQueryContext(action, query);

      new SequenceExclusiveAssertion(
        Allow.AllOf(Allow.PropertyGetters(), methodFilter)
        ).Assert(query.Result());
    }
  }
}
