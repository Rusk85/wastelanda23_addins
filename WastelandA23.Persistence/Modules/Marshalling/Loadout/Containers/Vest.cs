using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public class Vest : AbstractContainer
    {
        //public VestItem vestItems { get; private set; }
        public List<VestItem> vestItems { get; private set; }

        public Vest() { }
    }
}
