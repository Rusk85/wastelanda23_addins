using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public class Uniform : AbstractContainer
    {
        public List<Item> uniformItems { get; private set; }

        public Uniform() { }
    }
}
