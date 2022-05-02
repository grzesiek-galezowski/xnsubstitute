using System;
using NSubstitute.Core;
using NSubstitute.Core.SequenceChecking;

namespace TddXt.XNSubstitute.ImplementationDetails;

static internal class ExceptionMessage
{
  public static string For(CallSpecAndTarget[] querySpec, ICall[] receivedCalls,
    CallSpecAndTarget[] callsSpecifiedButNotReceived, ICall[] callsReceivedButNotSpecified, IQueryFilter queryFilter)
  {
    var sequenceFormatter = new SequenceFormatter("\n    ", querySpec, receivedCalls);

    var sequenceFormatterForUnexpectedAndExcessiveCalls = new SequenceFormatter("\n    ", callsSpecifiedButNotReceived,
      callsReceivedButNotSpecified);

    return String.Format("\nExpected to receive only these calls:\n{0}{1}\n\n"
                         + "Actually received the following calls:\n{0}{2}\n\n"
                         + "Calls expected but not received:\n{0}{3}\n\n"
                         + "Calls received but not expected:\n{0}{4}\n\n"
                         + "{5}\n\n"

      , "\n    "
      , sequenceFormatter.FormatQuery()
      , sequenceFormatter.FormatActualCalls()
      , sequenceFormatterForUnexpectedAndExcessiveCalls.FormatQuery()
      , sequenceFormatterForUnexpectedAndExcessiveCalls.FormatActualCalls()
      , $"*** Note: calls to {queryFilter.WhatIsFiltered} are not considered part of the query. ***"); //bug!
  }
}