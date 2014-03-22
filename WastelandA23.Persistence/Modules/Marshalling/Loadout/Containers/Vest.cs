using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public class Vest
    {

        public VestItems vestItems { get; private set; }

        public Vest() { }

        public Vest(VestItems vestItems)
        {
            this.vestItems = vestItems;
        }

    }
}
