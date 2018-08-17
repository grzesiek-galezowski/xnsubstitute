namespace TddXt.XFluentAssert.NSubstituteExtensions.ImplementationDetails
{
  using System;
  using System.Collections.Generic;
  using System.Runtime.Serialization;

  [Serializable]
  public class MultipleConditionsFailedException : Exception
  {

    public MultipleConditionsFailedException(Dictionary<int, Exception> exceptions)
      : base(exceptions.Count + " assertion(s) failed:" +
          Environment.NewLine + ShortExceptionsMessage(exceptions) + 
          Environment.NewLine + LongExceptionsMessage(exceptions))
    {
      Exceptions = exceptions;
    }

    protected MultipleConditionsFailedException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public Dictionary<int, Exception> Exceptions { get; }


    private static string ShortExceptionsMessage(Dictionary<int, Exception> exceptions)
    {
      var result = string.Empty;
      foreach (var exceptionEntry in exceptions)
      {
        result += $"{exceptionEntry.Key,3}) ";
        result += exceptionEntry.Value.Message + Environment.NewLine;
      }
      return result;
    }

    private static string LongExceptionsMessage(Dictionary<int, Exception> exceptions)
    {
      var result = string.Empty;
      foreach (var exceptionEntry in exceptions)
      {
        result += $"=== FAILED ASSERTION {exceptionEntry.Key} DETAILS ===" + Environment.NewLine;
        result += exceptionEntry.Value + Environment.NewLine;
      }
      return result;
    }
  }
}