using System.Reflection;

namespace TddXt.XNSubstitute.ImplementationDetails
{
  public interface IQueryFilter
  {
    bool ShouldVerify(MethodInfo methodInfo);
  }
}