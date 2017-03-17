// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Workaround to enable extension methods with .NET Framework 2.0
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class |
        AttributeTargets.Method)]
    sealed class ExtensionAttribute : Attribute { }
}
