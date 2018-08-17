namespace TddXt.XFluentAssert.NSubstituteExtensions.ImplementationDetails
{
  using System.Linq;

  using NSubstitute;
  using NSubstitute.Core;
  using NSubstitute.Core.SequenceChecking;

  public static class ReceivedNothingMessage
  {
    public static string For<T>(T substitute) where T : class
    {
      var formatter = new SequenceFormatter("\n    ", new CallSpecAndTarget[] {}, substitute.ReceivedCalls().ToArray());
      var message = "\nExpected to receive *no calls*.\n"
                    + $"Actually received the following calls:\n\n    {formatter.FormatActualCalls()}\n\n";
      return message;
    }
  }
}