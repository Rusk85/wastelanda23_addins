using System;

namespace WastelandA23.Marshalling
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false,
        Inherited = false)]
    public sealed class DerivedTypeAttribute : Attribute { }
}