﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Model.CodeFirstModel
{
    public class PrimaryWeapon
    {
        public int Id { get; set; }

        public string ClassName { get; set; }

        public virtual Magazine LoadedMagazine { get; set; }

        public virtual Loadout Loadout { get; set; }

        public virtual ICollection<PrimaryWeaponItem> PrimaryWeaponItems { get; set; }

    }
}
