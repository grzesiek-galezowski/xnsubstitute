using TddXt.XFluentAssert.Api;
using TddXt.XNSubstitute;

namespace DotNetCore2._0TestApp;

using System;
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;

public class Program
{
  public static void Main(string[] args)
  {
    Console.WriteLine("START");
    typeof(Program).Assembly.Should().NotHaveStaticFields();
    var enumerable = Substitute.For<ICollection<int>>();
    enumerable.ReceivedNothing();
    enumerable.Add(123);
    enumerable.Received().Add(Arg<int>.That(
      n => n.Should().Be(333),
      n => n.Should().Be(3453),
      n => new List<int> { n }.Should().BeLike(new List<int> { 3453 })));

    /*new object().Should().BeLike()
    new List<int> { 1, 2, 3 }.Should().BeLike(new List<int> { 1, 2, 3 });*/
    Console.WriteLine("END");
  }
}