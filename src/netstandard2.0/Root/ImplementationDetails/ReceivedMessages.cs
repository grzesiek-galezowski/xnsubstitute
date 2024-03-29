using System;
using System.Linq;
using NSubstitute;
using NSubstitute.Core;
using NSubstitute.Core.SequenceChecking;

namespace TddXt.XNSubstitute.ImplementationDetails;

public static class ReceivedMessages
{
  public static string ReceivedNothingMessageFor<T>(T substitute) where T : class
  {
    var formatter = new SequenceFormatter("\n    ", new CallSpecAndTarget[] {}, substitute.ReceivedCalls().ToArray());
    var message = "\nExpected to receive *no calls*.\n"
                  + $"Actually received the following calls:\n\n    {formatter.FormatActualCalls()}\n\n";
    return message;
  }

  public static string ReceivedNoCallsMatchingPredicateMessageFor<T>(T substitute, Func<ICall, bool> predicate, string descriptionOfConstraint) where T : class
  {
    var formatter = new SequenceFormatter("\n    ", new CallSpecAndTarget[] {}, substitute.ReceivedCalls().Where(predicate).ToArray());
    var message = $"\nExpected to receive *{descriptionOfConstraint}*.\n"
                  + $"Actually received the following calls:\n\n    {formatter.FormatActualCalls()}\n\n";
    return message;
  }

  public static string ReceivedDifferentCountThanExpectedCallsMessageFor<T>(T substitute, string descriptionOfConstraint) where T : class
  {
    var formatter = new SequenceFormatter("\n    ", new CallSpecAndTarget[] {}, substitute.ReceivedCalls().ToArray());
    var message = $"\nExpected to receive *{descriptionOfConstraint}*.\n"
                  + $"Actually received the following calls:\n\n    {formatter.FormatActualCalls()}\n\n";
    return message;
  }
}