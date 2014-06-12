using System.Collections.Generic;

namespace WastelandA23.Marshalling.Loadout
{
    public class SecondaryWeapon : AbstractWeapon
    {
        [ParamNumber(0)]
        public List<SecondaryWeaponItem> SecondaryWeaponItems { get; set; }

        [ParamNumber(1)]
        public override Magazine LoadedMagazine { get; set; }

        public SecondaryWeapon()
        {
        }
    }
}