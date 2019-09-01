using System.Reflection;
using NSubstitute.Core;

namespace TddXt.XNSubstitute.Root.ImplementationDetails
{
  public class FilterAllowingPropertyGetters : IQueryFilter
  {
    public bool ShouldVerify(MethodInfo methodInfo)
    {
      return methodInfo.GetPropertyFromGetterCallOrNull() == null;
    }
  }
}