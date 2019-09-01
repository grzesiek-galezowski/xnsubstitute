using System.Reflection;

namespace TddXt.XNSubstitute.Root.ImplementationDetails
{
  public interface IQueryFilter
  {
    bool ShouldVerify(MethodInfo methodInfo);
  }
}