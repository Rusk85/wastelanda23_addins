﻿using System.Collections.Generic;

namespace WastelandA23.Marshalling.Loadout
{
    public class Vest : AbstractContainer
    {
        //public VestItem vestItems { get; private set; }
        public List<Item> vestItems { get; private set; }

        public Vest()
        {
        }
    }
}