using System;
using System.Collections.Generic;
using System.Linq;

namespace TddXt.XNSubstitute.ImplementationDetails
{
  internal static class DiffAlgorithm
  {
    public static T2[] DifferenceBetween<T1, T2>(
      IEnumerable<T2> collection1,
      IEnumerable<T1> collection2, 
      Func<T2, T1, bool> matchCriteria) where T1 : class where T2 : class
    {
      var copyOfCollection2 = collection2.ToList();

      var notMatchedCalls = new List<T2>();

      foreach (var call in collection1)
      {
        var matchingSet2Element = copyOfCollection2.FirstOrDefault(spec => matchCriteria(call, spec));
        if (matchingSet2Element != null)
        {
          copyOfCollection2.Remove(matchingSet2Element);
        }
        else
        {
          notMatchedCalls.Add(call);
        }
      }

      return notMatchedCalls.ToArray();
    }
  }
}