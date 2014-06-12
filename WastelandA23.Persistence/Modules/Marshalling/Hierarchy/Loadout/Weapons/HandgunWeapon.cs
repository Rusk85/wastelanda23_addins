﻿using System.Collections.Generic;

namespace WastelandA23.Marshalling.Loadout
{
    public class HandgunWeapon : AbstractWeapon
    {
        [ParamNumber(0)]
        public List<HandgunWeaponItem> HandgunWeaponItems { get; set; }

        [ParamNumber(1)]
        public override Magazine LoadedMagazine { get; set; }

        public HandgunWeapon()
        {
        }
    }
}