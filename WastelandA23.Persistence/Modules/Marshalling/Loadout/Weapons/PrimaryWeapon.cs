using System.Collections.Generic;

namespace WastelandA23.Marshalling.Loadout
{
    public class PrimaryWeapon : AbstractWeapon
    {
        [ParamNumber(0)]
        public List<PrimaryWeaponItem> PrimaryWeaponItems { get; set; }

        [ParamNumber(1)]
        public override Magazine LoadedMagazine { get; set; }

        public PrimaryWeapon()
        {
        }
    }
}