using System;

namespace WastelandA23.Marshalling
{
    [AttributeUsageAttribute(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false,
    Inherited = true)]
    public sealed class ConsumeFlattenedAttribute : Attribute
    {
        public ConsumeFlattenedAttribute()
        {
        }
    }
}