using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.Core.DependencyInjection;
using NSubstitute.Exceptions;
using NSubstitute.Routing;
using TddXt.XNSubstitute;
using Xunit;
using Xunit.Sdk;

namespace TddXt.XFluentAssert.EndToEndSpecification;

public class XReceivedOnlyInOrderSpecification
{
  private readonly IFoo _foo;
  private readonly IBar _bar;

  public XReceivedOnlyInOrderSpecification()
  {
    _foo = Substitute.For<IFoo>();
    _bar = Substitute.For<IBar>();

  }

  [Fact]
  public void PassWhenCheckingASingleCall()
  {
    _foo.Start();
    XReceived.Exactly(() => _foo.Start());
  }

  [Fact]
  public void FailWhenCheckingASingleCallThatWasNotMade()
  {
    _foo.Start();

    Assert.Throws<CallSequenceNotFoundException>(() =>
      XReceived.Exactly(() => _foo.Finish())
    );
  }

  [Fact]
  public void PassWhenCallsMatchExactlyEvenInDifferentOrder()
  {
    _bar.End();
    _foo.Start(2);
    _foo.Finish();
    _bar.Begin();

    this.Invoking(_ =>
    {
      XReceived.Exactly(() =>
      {
        _foo.Start(2);
        _bar.Begin();
        _foo.Finish();
        _bar.End();
      });
    }).Should().ThrowExactly<CallSequenceNotFoundException>()
      .WithMessage(@"
Expected to receive these calls in order:

    XReceivedOnlyInOrderSpecification+IFoo.Start(2)
    XReceivedOnlyInOrderSpecification+IBar.Begin()
    XReceivedOnlyInOrderSpecification+IFoo.Finish()
    XReceivedOnlyInOrderSpecification+IBar.End()

Actually received matching calls in this order:

    XReceivedOnlyInOrderSpecification+IBar.End()
    XReceivedOnlyInOrderSpecification+IFoo.Start(2)
    XReceivedOnlyInOrderSpecification+IFoo.Finish()
    XReceivedOnlyInOrderSpecification+IBar.Begin()

*** Note: calls to property getters are not considered part of the query. ***"); //bug should depend on filter
  }

  [Fact]
  public void FailWhenCallArgDoesNotMatch()
  {
    _foo.Start(1);
    _bar.Begin();
    _foo.Finish();
    _bar.End();

    Assert.Throws<CallSequenceNotFoundException>(() =>
      XReceived.Exactly(() =>
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

    XReceived.Exactly(() =>
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
      XReceived.Exactly(() =>
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
      XReceived.Exactly(() =>
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
      XReceived.Exactly(() =>
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

    XReceived.Exactly(() =>
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


    XReceived.Exactly(() =>
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
      XReceived.Exactly(() =>
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

    XReceived.Exactly(() =>
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

    XReceived.Exactly(() =>
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
      XReceived.Exactly(() =>
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

    XReceived.Exactly(() =>
    {
      _foo.OnFoo += Arg.Any<Action>();
    });
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
      XReceived.Exactly(async () =>
      {
        substitute1.DoSomething();
        await substitute1.DoSomethingAsyncWithoutResult();
        substitute2.DoSomething();
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
      XReceived.Exactly(() =>
      {
        substitute1.DoSomething();
        substitute2.DoSomething();
        substitute1.DoSomethingAsyncWithoutResult();
      }, Allowing.Queries());
    }).Should().ThrowExactly<CallSequenceNotFoundException>(
      because: "verification of substitute2.DoSomethingAsyncWithoutResult() is missing")
      .WithMessage(@"
Expected to receive only these calls:

    1@XReceivedOnlyInOrderSpecification+IHaveCommandAndQueryAndTasks.DoSomething()
    2@XReceivedOnlyInOrderSpecification+IHaveCommandAndQueryAndTasks.DoSomething()
    1@XReceivedOnlyInOrderSpecification+IHaveCommandAndQueryAndTasks.DoSomethingAsyncWithoutResult()

Actually received the following calls:

    1@XReceivedOnlyInOrderSpecification+IHaveCommandAndQueryAndTasks.DoSomethingAsyncWithoutResult()
    1@XReceivedOnlyInOrderSpecification+IHaveCommandAndQueryAndTasks.DoSomething()
    2@XReceivedOnlyInOrderSpecification+IHaveCommandAndQueryAndTasks.DoSomethingAsyncWithoutResult()
    2@XReceivedOnlyInOrderSpecification+IHaveCommandAndQueryAndTasks.DoSomething()

Calls expected but not received:

    

Calls received but not expected:

    DoSomethingAsyncWithoutResult()

*** Note: calls to property getters and property getters and queries (methods returning something different than void and Task) are not considered part of the query. ***

");
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
