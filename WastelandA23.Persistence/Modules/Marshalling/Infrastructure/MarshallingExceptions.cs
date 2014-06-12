using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling
{
    public class MarshallingException : Exception
    {
        public MarshallingException(string Message) : base(Message)
        {

        }
    }

    public class AmbiguousMarshallingTypeException : MarshallingException
    {
        public AmbiguousMarshallingTypeException(string Message) : base(Message)
        {

        }
    }

    public class MissingOutputConverterFunctionException : MarshallingException
    {
        public MissingOutputConverterFunctionException(string Message)
            : base(Message)
        {

        }
    }

    public class InvalidSQFTypeException : MarshallingException
    {
        public InvalidSQFTypeException(string Message) : base(Message)
        {

        }
    }

}
