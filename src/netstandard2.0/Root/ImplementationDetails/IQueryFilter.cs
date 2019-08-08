using System.Reflection;

namespace TddXt.XNSubstitute.Root.ImplementationDetails
{
  public interface IQueryFilter
  {
    bool Allows(MethodInfo methodInfo);
  }
}