using System;
using NSubstitute;
using NSubstitute.Core;
using TddXt.XNSubstitute.ImplementationDetails;

namespace TddXt.XNSubstitute;

public static class XReceived
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
      DefualtFilterPlus(methodFilter)
    ).Assert(query.Result());
  }

  /// <summary>
  /// Combines <see cref="InOrder"/> and <see cref="Only"/>.
  /// </summary>
  /// <param name="action">Exactly these calls must've happened on these substitutes. Property getters are skipped</param>
  public static void Exactly(Action action)
  {
    Only(action);
    Received.InOrder(action);
  }

  /// <summary>
  /// Combines <see cref="InOrder"/> and <see cref="Only"/>.
  /// By default, skips property getters and allows specifying additional query filter
  /// </summary>
  /// <param name="action">Exactly these calls must've happened on these substitutes.</param>
  /// <param name="methodFilter">Additional call filter</param>
  public static void Exactly(Action action, IQueryFilter methodFilter)
  {
    var queryFilter = DefualtFilterPlus(methodFilter);
    Only(action, queryFilter);
    InOrder(action, queryFilter);
  }

  /// <summary>
  /// Works like the default <see cref="Received.InOrder" />, but allows specifying an additional query filter./>
  /// </summary>
  /// <param name="calls"></param>
  /// <param name="methodFilter"></param>
  public static void InOrder(Action calls, IQueryFilter methodFilter)
  {
    var query = new Query(SubstitutionContext.Current.CallSpecificationFactory);
    SubstitutionContext.Current.ThreadContext.RunInQueryContext(calls, query);
    new MySequenceInOrderAssertion(DefualtFilterPlus(methodFilter)).Assert(query.Result());
  }

  public static void Nothing(params object[] mocks)
  {
    foreach (var mock in mocks)
    {
      mock.ReceivedNothing();
    }
  }

  private static IQueryFilter DefualtFilterPlus(IQueryFilter methodFilter)
  {
    return Allowing.AllOf(Allowing.PropertyGetters(), methodFilter);
  }
}