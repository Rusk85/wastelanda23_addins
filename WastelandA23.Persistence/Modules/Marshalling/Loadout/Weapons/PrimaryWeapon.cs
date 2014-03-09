using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public class PrimaryWeapon : AbstractWeapon
    {

        public override List<LoadedMagazines> loadedMagazines { get; set; }
        public List<PrimaryWeaponItems> primaryWeaponItems { get; set; }

        public PrimaryWeapon(List<PrimaryWeaponItems> primaryWeaponItems,  List<LoadedMagazines> loadedMagazines)
        {
            this.primaryWeaponItems = primaryWeaponItems;
            this.loadedMagazines = loadedMagazines;
        }


    }
}
