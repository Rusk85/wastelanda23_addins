using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling
{
    [AttributeUsageAttribute(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true,
   Inherited = true)]
    public sealed class ParamNumberAttribute : Attribute
    {
        public int parameterIndex { get; set; }

        public Func<string, Object> converterFuncIn { get; set; }

        public Func<Object, string> converterFuncOut { get; set; }

        public ParamNumberAttribute(int index)
        {
            parameterIndex = index;
        }

        public ParamNumberAttribute(int index,
                                    Func<string, Object> converterIn)
            : this(index)
        {
            converterFuncIn = converterIn;
        }

        public ParamNumberAttribute(int index,
                                    Func<string, Object> converterIn,
                                    Func<Object, string> converterOut)
            : this(index, converterIn)
        {
            converterFuncOut = converterOut;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false,
    Inherited = false)]
    public sealed class DerivedTypeAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,
       AllowMultiple = false, Inherited = false)]
    public sealed class IgnoredMemberAttribute : Attribute { }

}
