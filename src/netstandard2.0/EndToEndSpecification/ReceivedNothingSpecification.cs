namespace TddXt.XFluentAssert.EndToEndSpecification
{
  using System;
  using System.Collections;

  using FluentAssertions;

  using NSubstitute;
  using NSubstitute.Exceptions;

  using TddXt.XFluentAssert.NSubstituteExtensions;

  using Xunit;

  public class ReceivedNothingSpecification
  {
    [Fact]
    public void ShouldPassWhenNoCallsWereMade()
    {
      var sub = Substitute.For<IEnumerable>();
      new Action(() => sub.ReceivedNothing()).Should().NotThrow();
    }

    [Fact]
    public void ShouldThrowWhenAnyCallsWereMade()
    {
      var sub = Substitute.For<IList>();
      sub.GetEnumerator();

      Assert.Throws<CallSequenceNotFoundException>(() => sub.ReceivedNothing());
    }
  }
}
