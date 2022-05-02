using System.Reflection;
using System.Threading.Tasks;
using TddXt.XNSubstitute.ImplementationDetails;

namespace TddXt.XNSubstitute;

public static class Allowing
{
  public static IQueryFilter PropertyGetters()
  {
    return new FilterAllowingPropertyGetters();
  }

  public static IQueryFilter AllOf(params IQueryFilter[] queryFilters)
  {
    return new CompoundFilter(queryFilters);
  }

  public static IQueryFilter Queries()
  {
    return new FilterAllowingQueries();
  }
}

public class FilterAllowingQueries : IQueryFilter
{
  public bool ShouldVerify(MethodInfo methodInfo)
  {
    return methodInfo.ReturnType == typeof(void) 
           || methodInfo.ReturnType == typeof(Task);
  }

  public string WhatIsFiltered => "queries (methods returning something different than void and Task)";
}