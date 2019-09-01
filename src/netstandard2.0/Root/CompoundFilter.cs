using System.Linq;
using System.Reflection;
using TddXt.XNSubstitute.Root.ImplementationDetails;

namespace TddXt.XNSubstitute.Root
{
  public class CompoundFilter : IQueryFilter
  {
    private readonly IQueryFilter[] _queryFilters;

    public CompoundFilter(IQueryFilter[] queryFilters)
    {
      _queryFilters = queryFilters;
    }

    public bool ShouldVerify(MethodInfo methodInfo)
    {
      return _queryFilters.All(f => f.ShouldVerify(methodInfo));
    }
  }
}