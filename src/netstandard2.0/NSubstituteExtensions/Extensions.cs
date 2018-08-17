namespace TddXt.XFluentAssert.NSubstituteExtensions
{
  using System.Linq;

  using NSubstitute;
  using NSubstitute.Exceptions;

  using TddXt.XFluentAssert.NSubstituteExtensions.ImplementationDetails;

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
