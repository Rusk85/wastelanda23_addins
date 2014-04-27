﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Model.CodeFirstModel
{
    public class Magazine
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public int Bullets { get; set; }

        public virtual HandgunWeapon HandgunWeapon { get; set; }
        public virtual PrimaryWeapon PrimaryWeapon { get; set; }
        public virtual SecondaryWeapon SecondaryWeapon { get; set; }
    }
}
