using System.Collections.Generic;

namespace WastelandA23.Marshalling.Loadout
{
    public class PrimaryWeapon : AbstractWeapon
    {
        [ParamNumber(0)]
        public List<PrimaryWeaponItems> primaryWeaponItems { get; set; }

        [ParamNumber(1)]
        public override Magazine loadedMagazines { get; set; }

        public PrimaryWeapon()
        {
        }
    }
}