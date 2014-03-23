using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public class SecondaryWeapon : AbstractWeapon
    {
        public override List<LoadedMagazines> loadedMagazines { get; set; }

        [ParamNumber(0)]
        public List<SecondaryWeaponItems> secondaryWeaponItems { get; set; }

        public SecondaryWeapon() { }

        public SecondaryWeapon(List<SecondaryWeaponItems> secondaryWeaponItems,
                               List<LoadedMagazines> loadedMagazines)
        {
            this.loadedMagazines = loadedMagazines;
            this.secondaryWeaponItems = secondaryWeaponItems;
        }
    }
}
