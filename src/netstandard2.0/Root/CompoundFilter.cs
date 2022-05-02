using System.Linq;
using System.Reflection;
using TddXt.XNSubstitute.ImplementationDetails;

namespace TddXt.XNSubstitute;

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

  public string WhatIsFiltered => string.Join(" and ", _queryFilters.Select(f => f.WhatIsFiltered).Distinct());
}