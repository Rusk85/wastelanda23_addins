using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public class CurrentWeapon
    {
        public string ClassName { get; set; }
        public CurrentMode currentMode { get; set; }


        public CurrentWeapon() { }
    }
}
