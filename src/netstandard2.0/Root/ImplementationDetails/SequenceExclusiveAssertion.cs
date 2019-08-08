namespace TddXt.XNSubstitute.Root.ImplementationDetails
{
  using System.Collections.Generic;
  using System.Linq;
  using NSubstitute;
  using NSubstitute.Core;
  using NSubstitute.Exceptions;

  public class SequenceExclusiveAssertion
  {
    private readonly IQueryFilter _queryFilter;

    public SequenceExclusiveAssertion(IQueryFilter queryFilter)
    {
      _queryFilter = queryFilter;
    }

    public void Assert(IQueryResults queryResult)
    {
      var querySpec = QuerySpecificationFrom(queryResult);
      var allReceivedCalls = AllCallsExceptPropertyGettersReceivedByTargetsOf(querySpec);

      var callsSpecifiedButNotReceived = GetCallsExpectedButNoReceived(querySpec, allReceivedCalls);
      var callsReceivedButNotSpecified = GetCallsReceivedButNotExpected(querySpec, allReceivedCalls);

      if (callsSpecifiedButNotReceived.Any() || callsReceivedButNotSpecified.Any())
      {
        throw new CallSequenceNotFoundException(ExceptionMessage.For(querySpec, allReceivedCalls,
          callsSpecifiedButNotReceived, callsReceivedButNotSpecified));
      }
    }

    private ICall[] AllCallsExceptPropertyGettersReceivedByTargetsOf(CallSpecAndTarget[] querySpec)
    {
      var allUniqueTargets = querySpec.Select(s => s.Target).Distinct();
      var allReceivedCalls = allUniqueTargets.SelectMany(target => target.ReceivedCalls());
      return allReceivedCalls.Where(x => _queryFilter.Allows(x.GetMethodInfo())).ToArray();
    }

    private CallSpecAndTarget[] QuerySpecificationFrom(IQueryResults queryResult)
    {
      return
        queryResult.QuerySpecification()
          .Where(x => _queryFilter.Allows(x.CallSpecification.GetMethodInfo()))
          .ToArray();
    }

    private static bool Matches(ICall call, CallSpecAndTarget specAndTarget)
    {
      if (object.ReferenceEquals(call.Target(), specAndTarget.Target))
        return specAndTarget.CallSpecification.IsSatisfiedBy(call);
      return false;
    }

    private static ICall[] GetCallsReceivedButNotExpected(IEnumerable<CallSpecAndTarget> expectedCalls, IEnumerable<ICall> receivedCalls)
    {
      return DiffAlgorithm.DifferenceBetween(receivedCalls, expectedCalls, Matches);
    }

    private static CallSpecAndTarget[] GetCallsExpectedButNoReceived(IEnumerable<CallSpecAndTarget> expectedCalls, IEnumerable<ICall> receivedCalls)
    {
      return DiffAlgorithm.DifferenceBetween(expectedCalls, receivedCalls, (call, spec) => Matches(spec, call));
    }
  }
}