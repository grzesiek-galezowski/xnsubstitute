using System.Reflection;
using TddXt.XNSubstitute.Root.ImplementationDetails;

namespace TddXt.XNSubstitute.Root
{
  public static class Allow
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
    public bool Allows(MethodInfo methodInfo)
    {
      return methodInfo.ReturnType == typeof(void);
    }
  }
}