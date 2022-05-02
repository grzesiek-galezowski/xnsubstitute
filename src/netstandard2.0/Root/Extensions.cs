using System;
using System.Linq;
using NSubstitute;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.ReceivedExtensions;
using TddXt.XNSubstitute.ImplementationDetails;
using static TddXt.XNSubstitute.ReceivedCallsConstraint;

namespace TddXt.XNSubstitute;

public class ReceivedCallsConstraint
{
  public ReceivedCallsConstraint(Func<ICall, bool> failWhenMatched, string constraintDescription)
  {
    FailWhenMatched = failWhenMatched;
    ConstraintDescription = constraintDescription;
  }

  public Func<ICall, bool> FailWhenMatched { get; }
  public string ConstraintDescription { get; }

  public static ReceivedCallsConstraint Whatsoever()
  {
    return new ReceivedCallsConstraint(c => true, "no calls");
  }

  public static ReceivedCallsConstraint ThatAreCommands()
  {
    return new ReceivedCallsConstraint(c => c.GetReturnType() == typeof(void), "no commands");
  }

  public static ReceivedCallsConstraint Whatsoever(object mock)
  {
    return new ReceivedCallsConstraint(c => true, "no calls to " + mock);
  }

  public static ReceivedCallsConstraint ThatAreCommands(object mock)
  {
    return new ReceivedCallsConstraint(c => c.GetReturnType() == typeof(void), "no commands to " + mock);
  }
}

public static class Extensions
{
  public static T ReceivedNothing<T>(this T substitute) where T : class
  {
    return substitute.ReceivedNoCalls(Whatsoever(substitute));
  }

  public static T ReceivedNoCommands<T>(this T substitute) where T : class
  {
    return substitute.ReceivedNoCalls(ThatAreCommands(substitute));
  }

  public static T ReceivedNoCalls<T>(this T substitute, ReceivedCallsConstraint receivedCallsConstraint) where T : class
  {
    if (substitute.ReceivedCalls().Any(receivedCallsConstraint.FailWhenMatched))
    {
      var message = ReceivedMessages.ReceivedNoCallsMatchingPredicateMessageFor(substitute, receivedCallsConstraint.FailWhenMatched, receivedCallsConstraint.ConstraintDescription);
      throw new CallSequenceNotFoundException(message);
    }

    return substitute;
  }

  public static T ReceivedOnly<T>(this T substitute) where T : class
  {
    return substitute.ReceivedOnly(Quantity.AtLeastOne());
  }

  public static T ReceivedOnly<T>(this T substitute, int requiredNumberOfCalls) where T : class
  {
    return ReceivedOnly(substitute, Quantity.Exactly(requiredNumberOfCalls));
  }

  public static T ReceivedOnly<T>(this T substitute, Quantity requiredQuantity) where T : class
  {
    if(!requiredQuantity.Matches(substitute.ReceivedCalls()))
    {
      throw new ReceivedCallsException( 
        ReceivedMessages.ReceivedDifferentCountThanExpectedCallsMessageFor(
          substitute, 
          requiredQuantity.Describe("call total on this substitute","calls total on this substitute")));
    }

    return substitute.Received(requiredQuantity);
  }
}