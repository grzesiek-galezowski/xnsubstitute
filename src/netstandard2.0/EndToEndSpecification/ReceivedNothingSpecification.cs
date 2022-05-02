using System.Collections;

using FluentAssertions;

using NSubstitute;
using NSubstitute.Exceptions;
using TddXt.XNSubstitute;
using Xunit;

namespace TddXt.XFluentAssert.EndToEndSpecification;

public class ReceivedNothingSpecification
{
  [Fact]
  public void ShouldPassWhenNoCallsWereMade()
  {
    var sub = Substitute.For<IEnumerable>();
    sub.Invoking(s => s.ReceivedNothing()).Should().NotThrow();
  }

  [Fact]
  public void ShouldThrowWhenAnyCallsWereMade()
  {
    var sub = Substitute.For<IList>();
    sub.GetEnumerator();

    Assert.Throws<CallSequenceNotFoundException>(() => sub.ReceivedNothing());
  }

  [Fact]
  public void ShouldNotThrowWhenReceivedOnlyQueries()
  {
    var sub = Substitute.For<IList>();
    sub.GetEnumerator();

    sub.Invoking(s => s.ReceivedNoCommands()).Should().NotThrow();
  }

  [Fact]
  public void ShouldThrowWhenReceivedCommand()
  {
    var sub = Substitute.For<IList>();
    sub.Clear();

    sub.Invoking(s => s.ReceivedNoCommands()).Should().ThrowExactly<CallSequenceNotFoundException>();
  }
}