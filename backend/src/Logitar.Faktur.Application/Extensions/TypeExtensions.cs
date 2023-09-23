namespace Logitar.Faktur.Application.Extensions;

public static class TypeExtensions
{
  public static string GetLongestName(this Type type) => type.AssemblyQualifiedName ?? type.FullName ?? type.Name;
}
