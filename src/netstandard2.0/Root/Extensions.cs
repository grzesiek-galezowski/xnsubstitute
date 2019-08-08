using System;
using NSubstitute.Core;
using static TddXt.XNSubstitute.Root.ReceivedCallsConstraint;

namespace TddXt.XNSubstitute.Root
{
  using System.Linq;
  using NSubstitute;
  using NSubstitute.Exceptions;
  using ImplementationDetails;

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
  }

  public static class Extensions
  {
    public static T ReceivedNothing<T>(this T substitute) where T : class
    {
      return substitute.ReceivedNoCalls(Whatsoever());
    }

    public static T ReceivedNoCommands<T>(this T substitute) where T : class
    {
      return substitute.ReceivedNoCalls(ThatAreCommands());
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
  }
}
