using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public class SecondaryWeapon : AbstractWeapon
    {

        [ParamNumber(0)]
        public List<SecondaryWeaponItems> secondaryWeaponItems { get; set; }

        [ParamNumber(1)]
        public override LoadedMagazine loadedMagazines { get; set; }

        public SecondaryWeapon() { }

        public SecondaryWeapon(List<SecondaryWeaponItems> secondaryWeaponItems,
                               LoadedMagazine loadedMagazines)
        {
            this.loadedMagazines = loadedMagazines;
            this.secondaryWeaponItems = secondaryWeaponItems;
        }
    }
}
