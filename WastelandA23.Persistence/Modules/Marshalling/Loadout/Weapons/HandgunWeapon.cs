using System.Collections.Generic;

namespace WastelandA23.Marshalling.Loadout
{
    public class HandgunWeapon : AbstractWeapon
    {
        [ParamNumber(0)]
        public List<HandgunItems> handgunItems { get; set; }

        [ParamNumber(1)]
        public override Magazine loadedMagazines { get; set; }

        public HandgunWeapon()
        {
        }
    }
}