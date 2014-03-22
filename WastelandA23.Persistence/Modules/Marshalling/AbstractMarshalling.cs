using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public abstract class AbstractMarshalling
    {
        [ParamNumber(0)]
        public string Command { get; private set; }

        public AbstractMarshalling() { }

        public AbstractMarshalling(string inCommand)
        {
            Command = inCommand;
        }
    }
}
