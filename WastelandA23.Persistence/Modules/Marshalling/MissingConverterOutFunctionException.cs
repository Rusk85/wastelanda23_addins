using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling
{
    public class MissingConverterOutFunctionException : Exception
    {
        public MissingConverterOutFunctionException(string Message) : base(Message)
        {

        }
    }
}
