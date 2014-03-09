using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    abstract class AbstractMarshalling
    {


        public string Command { get; private set; }

        public AbstractMarshalling(string inCommand)
        {
            Command = inCommand;
        }

    }
}
