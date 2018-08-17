namespace TddXt.XNSubstitute.Root
{
  using System;

  public static class Arg<T>
  {
    public static T That(params Action<T>[] assertions)
    {
      return XArg.Where(assertions);
    }
  }
}