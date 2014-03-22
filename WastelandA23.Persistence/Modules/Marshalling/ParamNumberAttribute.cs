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
        public int parameterIndex { get; set;  }
        public Func<string, Object> converterFunc { get; set; }

        public ParamNumberAttribute(int index)
        {
            parameterIndex = index;
        }

        public ParamNumberAttribute(int index, Func<string, Object> converter) : this(index)
        {
            converterFunc = converter;
        }
    }
}
