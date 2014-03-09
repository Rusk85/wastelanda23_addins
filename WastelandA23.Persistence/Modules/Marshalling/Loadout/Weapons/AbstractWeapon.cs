using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public abstract class AbstractWeapon
    {

        public abstract List<LoadedMagazines> loadedMagazines { get; set; }
        public string ClassName { get; private set; }

    }
}
