﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public class PrimaryWeapon : AbstractWeapon
    {
        [ParamNumber(0)]
        public List<PrimaryWeaponItems> primaryWeaponItems { get; set; }
        [ParamNumber(1)]
        public override LoadedMagazine loadedMagazines { get; set; }
        

        public PrimaryWeapon() { }

        public PrimaryWeapon(List<PrimaryWeaponItems> primaryWeaponItems,  LoadedMagazine loadedMagazines)
        {
            this.primaryWeaponItems = primaryWeaponItems;
            this.loadedMagazines = loadedMagazines;
        }


    }
}
