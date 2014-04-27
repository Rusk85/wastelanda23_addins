using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public class Loadout
    {

        public List<AssignableItem> AssignedItems { get; private set; }
        public PrimaryWeapon PrimaryWeapon { get; private set; }
        public HandgunWeapon HandgunWeapon { get; private set; }
        public SecondaryWeapon SecondaryWeapon { get; private set; }
        public Uniform Uniform { get; private set; }
        public Vest Vest { get; private set; }
        public Backpack Backpack { get; private set; }
        
        public Loadout() { }

    }
}
