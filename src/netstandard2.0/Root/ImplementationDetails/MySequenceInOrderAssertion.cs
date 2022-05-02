using System.Linq;
using NSubstitute.Core;
using NSubstitute.Core.SequenceChecking;
using NSubstitute.Exceptions;

namespace TddXt.XNSubstitute.ImplementationDetails;

internal class MySequenceInOrderAssertion
{
  private readonly IQueryFilter _queryFilter;

  public MySequenceInOrderAssertion(IQueryFilter queryFilter)
  {
    _queryFilter = queryFilter;
  }

  public void Assert(IQueryResults queryResult)
  {
    var array1 = queryResult.MatchingCallsInOrder().Where(x => _queryFilter.ShouldVerify(x.GetMethodInfo())).ToArray();
    var array2 = queryResult.QuerySpecification().Where(x => _queryFilter.ShouldVerify(x.CallSpecification.GetMethodInfo())).ToArray();
    if (array1.Length != array2.Length)
    {
      throw new CallSequenceNotFoundException(GetExceptionMessage(array2, array1));
    }
    if (array1.Zip(array2, (call, specAndTarget) => new
        {
          Call = call,
          Spec = specAndTarget.CallSpecification,
          IsMatch = Matches(call, specAndTarget)
        }).Any(x => !x.IsMatch))
      throw new CallSequenceNotFoundException(GetExceptionMessage(array2, array1));
  }

  private static bool Matches(ICall call, CallSpecAndTarget specAndTarget) 
    => call.Target() == specAndTarget.Target && specAndTarget.CallSpecification.IsSatisfiedBy(call);

  private string GetExceptionMessage(CallSpecAndTarget[] querySpec, ICall[] matchingCallsInOrder)
  {
    return ExceptionMessage.For(querySpec, matchingCallsInOrder, querySpec, matchingCallsInOrder, _queryFilter);
  }
}