﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public abstract class AbstractWeapon
    {
        public string ClassName { get; private set; }
        public abstract LoadedMagazines loadedMagazines { get; set; }
    }
}
