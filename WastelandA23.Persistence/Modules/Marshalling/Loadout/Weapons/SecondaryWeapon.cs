using System.Collections.Generic;

namespace WastelandA23.Marshalling.Loadout
{
    public class SecondaryWeapon : AbstractWeapon
    {
        [ParamNumber(0)]
        public List<SecondaryWeaponItems> secondaryWeaponItems { get; set; }

        [ParamNumber(1)]
        public override Magazine loadedMagazines { get; set; }

        public SecondaryWeapon()
        {
        }
    }
}