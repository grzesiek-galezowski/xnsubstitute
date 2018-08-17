namespace TddXt.XNSubstitute.Root
{
  using System.Linq;

  using NSubstitute;
  using NSubstitute.Exceptions;

  using TddXt.XNSubstitute.Root.ImplementationDetails;

  public static class Extensions
  {
    public static T ReceivedNothing<T>(this T substitute) where T : class
    {
      if (substitute.ReceivedCalls().Count() != 0)
      {
        var message = ReceivedNothingMessage.For(substitute);
        throw new CallSequenceNotFoundException(message);
      }
      return substitute;
    }
  }
}
