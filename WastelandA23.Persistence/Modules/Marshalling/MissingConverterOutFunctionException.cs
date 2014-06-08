using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling
{
    public class MissingOutputConverterFunctionException : Exception
    {
        public MissingOutputConverterFunctionException(string Message) : base(Message)
        {

        }
    }
}
