using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public class Uniform
    {

        public UniformItems uniformItems { get; private set; }

        public Uniform() { }

        public Uniform(UniformItems uniformItems)
        {
            this.uniformItems = uniformItems;
        }


    }
}
