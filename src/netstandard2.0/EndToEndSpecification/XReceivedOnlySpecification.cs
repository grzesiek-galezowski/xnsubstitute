using System.Threading.Tasks;
using FluentAssertions;
using TddXt.XNSubstitute;
using System;
using NSubstitute;
using NSubstitute.Exceptions;
using NSubstitute.ReceivedExtensions;
using Xunit;

namespace TddXt.XFluentAssert.EndToEndSpecification;

public class XReceivedOnlySpecification
{
  private readonly IFoo _foo;
  private readonly IBar _bar;

  public XReceivedOnlySpecification()
  {
    _foo = Substitute.For<IFoo>();
    _bar = Substitute.For<IBar>();
  }

  [Fact]
  public void PassWhenCheckingASingleCall()
  {
    _foo.Start();
    XReceived.Only(() => _foo.Start());
  }

  [Fact]
  public void FailWhenCheckingASingleCallThatWasNotMade()
  {
    _foo.Start();

    Assert.Throws<CallSequenceNotFoundException>(() =>
      XReceived.Only(() => _foo.Finish())
    );
  }

  [Fact]
  public void PassWhenCallsMatchExactlyEvenInDifferentOrder()
  {
    _bar.End();
    _foo.Start(2);
    _foo.Finish();
    _bar.Begin();

    XReceived.Only(() =>
    {
      _foo.Start(2);
      _bar.Begin();
      _foo.Finish();
      _bar.End();
    });
  }

  [Fact]
  public void FailWhenCallArgDoesNotMatch()
  {
    _foo.Start(1);
    _bar.Begin();
    _foo.Finish();
    _bar.End();

    Assert.Throws<CallSequenceNotFoundException>(() =>
      XReceived.Only(() =>
      {
        _foo.Start(2);
        _bar.Begin();
        _foo.Finish();
        _bar.End();
      })
    );
  }

  [Fact]
  public void UseArgMatcher()
  {
    _foo.Start(1);
    _bar.Begin();
    _foo.Finish();
    _bar.End();

    XReceived.Only(() =>
    {
      _foo.Start(Arg.Is<int>(x => x < 10));
      _bar.Begin();
      _foo.Finish();
      _bar.End();

    });
  }

  [Fact]
  public void FailWhenOneOfTheCallsWasNotReceived()
  {
    _foo.Start();
    _foo.Finish();

    Assert.Throws<CallSequenceNotFoundException>(() =>
      XReceived.Only(() =>
      {
        _foo.Start();
        _foo.FunkyStuff("hi");
        _foo.Finish();
      })
    );
  }

  [Fact]
  public void FailWhenAdditionalRelatedCallsAtStart()
  {
    _foo.Start();
    _foo.Start();
    _bar.Begin();
    _foo.Finish();
    _bar.End();

    Assert.Throws<CallSequenceNotFoundException>(() =>
      XReceived.Only(() =>
      {
        _foo.Start();
        _bar.Begin();
        _foo.Finish();
        _bar.End();
      })
    );
  }

  [Fact]
  public void FailWhenAdditionalRelatedCallsAtEnd()
  {
    _foo.Start();
    _bar.Begin();
    _foo.Finish();
    _bar.End();
    _foo.Start();

    Assert.Throws<CallSequenceNotFoundException>(() =>
      XReceived.Only(() =>
      {
        _foo.Start();
        _bar.Begin();
        _foo.Finish();
        _bar.End();
      })
    );
  }

  [Fact]
  public void IgnoreCallsFromUnrelatedSubstitutes()
  {
    _bar.Begin();
    _foo.FunkyStuff("get funky!");
    _bar.End();

    XReceived.Only(() =>
    {
      _bar.Begin();
      _bar.End();
    });
  }


  [Fact]
  public void CheckAutoSubbedProps()
  {
    _foo.Start();
    _bar.Baz.Flurgle = "hi";
    _foo.Finish();


    XReceived.Only(() =>
    {
      _foo.Start();
      _bar.Baz.Flurgle = "hi";
      _foo.Finish();
    });
  }

  [Fact]
  public void FailWhenAutoSubbedPropCallNotReceived()
  {
    _foo.Start();
    _bar.Baz.Flurgle = "hi";
    _foo.Finish();


    Assert.Throws<CallSequenceNotFoundException>(() =>
      XReceived.Only(() =>
      {
        _foo.Start();
        _bar.Baz.Flurgle = "howdy";
        _foo.Finish();
      })
    );
  }

  [Fact]
  public void VerifyingCallsShouldIgnorePropertyGetterCalls()
  {
    var baz = _bar.Baz;
    baz.Wurgle();
    baz.Slurgle();

    XReceived.Only(() =>
    {
      // This call spec should be regarded as matching the
      // calling code above. So needs to ignore the get 
      // request to _bar.Baz.
      _bar.Baz.Wurgle();
      _bar.Baz.Slurgle();
    });
  }

  [Fact]
  public void OrderedCallsWithDelegates()
  {
    var func = Substitute.For<Func<string>>();
    func();
    func();

    XReceived.Only(() =>
    {
      func();
      func();
    });
  }

  [Fact]
  public void NonMatchingOrderedCallsWithDelegates()
  {
    var func = Substitute.For<Action>();
    func();

    Assert.Throws<CallSequenceNotFoundException>(() =>
      XReceived.Only(() =>
      {
        func();
        func();
      })
    );
  }

  [Fact]
  public void EventSubscription()
  {
    _foo.OnFoo += () => { };

    XReceived.Only(() => { _foo.OnFoo += Arg.Any<Action>(); });
  }

  [Fact]
  public async Task ShouldNotCheckQueriesWhenTheyAreAllowed() //bug this should not pass!!!
  {
    var substitute1 = Substitute.For<IHaveCommandAndQueryAndTasks>();
    var substitute2 = Substitute.For<IHaveCommandAndQueryAndTasks>();

    substitute1.DoSomething();
    await substitute1.DoSomethingAsyncWithoutResult();
    var result1 = substitute1.QuerySomething();
    var result2 = await substitute1.QuerySomethingAsync();
    substitute2.DoSomething();
    await substitute2.DoSomethingAsyncWithoutResult();
    await substitute2.DoSomethingAsyncWithResult();
    var result3 = substitute2.QuerySomething();
    var result4 = await substitute2.QuerySomethingAsync();

    new Action(() =>
    {
      XReceived.Only(async () =>
      {
        substitute1.DoSomething();
        substitute2.DoSomething();
        await substitute1.DoSomethingAsyncWithoutResult();
        await substitute2.DoSomethingAsyncWithoutResult();
      }, Allowing.Queries());
    }).Should().NotThrow();
  }

  [Fact]
  public async Task ShouldNotCheckQueriesWhenTheyAreAllowed2()
  {
    var substitute1 = Substitute.For<IHaveCommandAndQueryAndTasks>();
    var substitute2 = Substitute.For<IHaveCommandAndQueryAndTasks>();

    await substitute1.DoSomethingAsyncWithoutResult();
    await substitute1.DoSomethingAsyncWithResult();
    substitute1.DoSomething();
    await substitute2.DoSomethingAsyncWithoutResult();
    substitute2.DoSomething();

    new Action(() =>
    {
      XReceived.Only(() =>
      {
        substitute1.DoSomething();
        substitute2.DoSomething();
        substitute1.DoSomethingAsyncWithoutResult();
      }, Allowing.Queries());
    }).Should().Throw<Exception>(because: "verification of substitute2.DoSomethingAsyncWithoutResult() is missing");
  }

  public interface IFoo
  {
    void Start();
    void Start(int i);
    void Finish();
    void FunkyStuff(string s);
    event Action OnFoo;
  }

  public interface IBar
  {
    void Begin();
    void End();
    IBaz Baz { get; }
  }

  public interface IBaz
  {
    string Flurgle { get; set; }
    void Wurgle();
    void Slurgle();
  }

  public interface IHaveCommandAndQueryAndTasks
  {
    void DoSomething();
    Task DoSomethingAsyncWithoutResult();
    Task<int> DoSomethingAsyncWithResult();
    string QuerySomething();
    Task<string> QuerySomethingAsync();
  }
}

public class XReceivedOnly2Specification
{
  [Fact]
  public void ShouldXXX() //bug
  {
    //GIVEN
    var something1 = Substitute.For<ISomething>();
    var something2 = Substitute.For<ISomething>();

    //WHEN
    something1.Do1();
    something1.Do1();
    something2.Do1();

    //THEN
    something1.Invoking(s => s.ReceivedOnly(Quantity.Exactly(1)).Do1())
      .Should().ThrowExactly<ReceivedCallsException>()
      .WithMessage(@"
Expected to receive *exactly 1 call total on this substitute*.
Actually received the following calls:

    Do1()
    Do1()

");
    something2.Invoking(s => s.ReceivedOnly(Quantity.Exactly(1)).Do1()).Should().NotThrow();
  }

  public interface ISomething
  {
    public void Do1();
    public void Do2();
  }
}