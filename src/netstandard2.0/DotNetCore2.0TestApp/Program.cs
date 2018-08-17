namespace DotNetCore2._0TestApp
{
  using System;
  using System.Collections;
  using System.Collections.Generic;

  using FluentAssertions;

  using TddXt.XFluentAssert.Root;

  public class Program
  {
    public static void Main(string[] args)
    {
      Console.WriteLine("START");
      typeof(Program).Assembly.Should().NotHaveStaticFields();
      //var enumerable = Substitute.For<ICollection<int>>();
      //todo move that to new API: enumerable.ReceivedNothing().
      //enumerable.Received().
      /*new object().Should().BeLike()
      new List<int> { 1, 2, 3 }.Should().BeLike(new List<int> { 1, 2, 3 });*/
      Console.WriteLine("END");
    }
  }
}
