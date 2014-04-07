using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public class Loadout
    {

        //public AssignedItems assigendItems { get; private set; }
        //TODO: List<AsssignableItem> assignedItems
        public List<AssignableItem> assigendItems { get; private set; }
        //List<AbstractWeapon> weapons ?
        public PrimaryWeapon primaryWeapon { get; private set; }
        public HandgunWeapon handgunWeapon { get; private set; }
        public SecondaryWeapon secondaryWeapon { get; private set; }
        public Uniform uniform { get; private set; }
        public Vest vest { get; private set; }
        public Backpack backpack { get; private set; }
        
        public Loadout() { }

        /*
        Loadout(AssignedItems assigendItems,
                PrimaryWeapon primaryWeapon,
                SecondaryWeapon secondaryWeapon,
                Uniform uniform,
                Vest vest,
                Backpack backpack)
        {
            this.assigendItems = assigendItems;
            this.primaryWeapon = primaryWeapon;
            this.secondaryWeapon = secondaryWeapon;
            this.uniform = uniform;
            this.vest = vest;
            this.backpack = backpack;
        }
         */

    }
}
