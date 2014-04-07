using System;

namespace WastelandA23.Marshalling
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false, Inherited = false)]
    public sealed class IgnoredMemberAttribute : Attribute { }
}