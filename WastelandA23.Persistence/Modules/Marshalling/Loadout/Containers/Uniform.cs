using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    class Uniform
    {

        public UniformItems uniformItems { get; private set; }

        public Uniform(UniformItems uniformItems)
        {
            this.uniformItems = uniformItems;
        }


    }
}
