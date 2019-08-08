using System.Reflection;
using NSubstitute.Core;

namespace TddXt.XNSubstitute.Root.ImplementationDetails
{
  public class FilterAllowingPropertyGetters : IQueryFilter
  {
    public bool Allows(MethodInfo methodInfo)
    {
      return methodInfo.GetPropertyFromGetterCallOrNull() == null;
    }
  }
}