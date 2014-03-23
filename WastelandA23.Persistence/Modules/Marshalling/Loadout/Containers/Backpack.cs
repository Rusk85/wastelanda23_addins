using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public class Backpack : AbstractContainer
    {
        public List<BackpackItem> backPackItems { get; set; }
    }
}
