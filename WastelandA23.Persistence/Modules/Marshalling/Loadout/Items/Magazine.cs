using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public class Magazine : Item
    {
        [ParamNumber(0)]
        public string Bullets { get; set; }

        public Magazine() : base() { } 

    }
}
