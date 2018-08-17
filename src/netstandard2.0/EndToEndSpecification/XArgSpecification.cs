namespace TddXt.XFluentAssert.EndToEndSpecification.NSubstituteSpecifications
{
  using System.Collections.Generic;

  using FluentAssertions;

  using NSubstitute;
  using NSubstitute.Exceptions;

  using TddXt.XFluentAssert.NSubstituteExtensions;

  using Xunit;

  public class XArgSpecification
  {
    [Fact]
    public void ShouldCorrectlyReportLikenessWithNSubstitute()
    {
      var s = Substitute.For<IXyz>();
      s.Do(new List<int>());

      s.Received(1).Do(XArg.IsLike(new List<int>()));
      s.DidNotReceive().Do(XArg.IsLike(new List<int> { 1 }));
    }

    [Fact]
    public void ShouldCorrectlyReportUnlikenessWithNSubstitute()
    {
      var s = Substitute.For<IXyz>();
      s.Do(new List<int>());

      var e = Assert.Throws<ReceivedCallsException>(() =>
      {
        s.Received(1).Do(XArg.IsNotLike(new List<int>()));
      });

      Assert.Throws<ReceivedCallsException>(() =>
      {
        s.DidNotReceive().Do(XArg.IsNotLike(new List<int> {1}));
      });
    }

    [Fact]
    public void ShouldCorrectlyReportCollectionEquivalency()
    {
      var s = Substitute.For<IXyz>();
      s.Do(new List<int>());
      s.Received(1).Do(XArg.EquivalentTo(new List<int>()));
    }

    [Fact]
    public void ShouldCorrectlyReportCollectionEquivalencyError()
    {
      var s = Substitute.For<IXyz>();
      s.Do(new List<int>());
      Assert.Throws<ReceivedCallsException>(() =>
      {
        s.Received(1).Do(XArg.EquivalentTo(new List<int>() {1, 2, 3}));
      });
    }

    [Fact]
    public void ShouldCorrectlyReportCollectionSequenceEquality()
    {
      var s = Substitute.For<IXyz>();
      s.Do(new List<int>() {1,2,3});
      s.Do(new List<string>() {"a", "b", "c"});
      s.Received(1).Do(XArg.SequenceEqualTo(new List<int>() {1,2,3}));
      s.Received(1).Do(XArg.SequenceEqualTo(new List<string>() {"a", "b", "c"}));
    }

    [Fact]
    public void ShouldCorrectlyReportCollectionSequenceUnequality()
    {
      var s = Substitute.For<IXyz>();
      s.Do(new List<int>() {1,2,3});
      s.Do(new List<string>() {"a", "b", "c"});
      s.Received(1).Do(XArg.NotSequenceEqualTo(new List<int>() {1,2,3,4}));
      s.Received(1).Do(XArg.NotSequenceEqualTo(new List<string>() {"a", "b", "c", "d"}));
    }

    [Fact]
    public void ShouldLetArgumentPassWhenItPassesSpecifiedAssertions()
    {
      //GIVEN
      var xyz = Substitute.For<IXyz>();

      //WHEN
      xyz.Do(new List<int>() { 1,2,3 });
      xyz.Do(new List<int>() { 6,5,4 });

      //THEN
      xyz.Received(1).Do(XArg.Passing<List<int>>(
        l => l.Should().BeInAscendingOrder(),
        l => l.Should().Contain(1),
        l => l.Should().Contain(2),
        l => l.Should().Contain(3)));

      xyz.Received(1).Do(XArg.Passing<List<int>>(
        l => l.Should().BeInDescendingOrder(),
        l => l.Should().Contain(6),
        l => l.Should().Contain(5),
        l => l.Should().Contain(4)));
    }

    [Fact]
    public void ShouldNotLetArgumentPassWhenItDoesNotPassSpecifiedAssertions()
    {
      //GIVEN
      var xyz = Substitute.For<IXyz>();

      //WHEN
      xyz.Do(new List<int>() { 1,2,3 });

      //THEN
      var exception = Assert.Throws<ReceivedCallsException>(() =>
      {
        xyz.Received(1).Do(XArg.Passing<List<int>>(
          l => l.Should().BeInDescendingOrder(),
          l => l.Should().Contain(4),
          l => l.Should().Contain(5),
          l => l.Should().Contain(1),
          l => l.Should().Contain(6)));
      });

      exception.Message.Should().Contain("4 assertion(s) failed");
      exception.Message.Should().Contain("=== FAILED ASSERTION 1 DETAILS ===");
      exception.Message.Should().Contain("=== FAILED ASSERTION 2 DETAILS ===");
      exception.Message.Should().Contain("=== FAILED ASSERTION 3 DETAILS ===");
      exception.Message.Should().Contain("=== FAILED ASSERTION 5 DETAILS ===");
      exception.Message.Should().Contain("Expected l to contain items in descending order, but found {1, 2, 3} where item at index 0 is in wrong order");
      exception.Message.Should().Contain("Expected l {1, 2, 3} to contain 4");
      exception.Message.Should().Contain("Expected l {1, 2, 3} to contain 5");
      exception.Message.Should().Contain("Expected l {1, 2, 3} to contain 6");
      exception.Message.Should().NotContain("=== FAILED CONDITION 4 ===");
    }
  }

  public interface IXyz
  {
    void Do(IEnumerable<int> ints); 
    void Do(List<string> strings); 
    void Do2(int x, int y); 
  }

}
