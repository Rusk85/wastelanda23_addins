using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public class CurrentLoadedMagazines
    {

        public LoadedMagazines primaryWeaponMagazine { get; set; }
        public LoadedMagazines handgunWeaponMagazine { get; set; }
        public LoadedMagazines secondaryWeaponMagazine { get; set; }
        public LoadedMagazines otherWeaponMagazine { get; set; }

        public CurrentLoadedMagazines() { }

    }
}
