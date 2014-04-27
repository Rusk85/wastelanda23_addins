﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Model.CodeFirstModel
{
    public class SecondaryWeaponItem
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public int SecondaryWeaponId {get; set;}
        public virtual SecondaryWeapon SecondaryWeapon { get; set; }
    }
}
