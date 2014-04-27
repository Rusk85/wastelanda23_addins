using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Model.CodeFirstModel
{
    public class Loadout
    {
        public int Id { get; set; }

        public virtual PrimaryWeapon PrimaryWeapon { get; set; }
        public virtual SecondaryWeapon SecondaryWeapon { get; set; }
        public virtual HandgunWeapon HandgunWeapon { get; set; }

        public virtual Backpack Backpack { get; set; }
        public virtual Uniform Uniform { get; set; }
        public virtual Vest Vest { get; set; }

        public ICollection<AssignableItem> AssignableItems { get; set; }

        public virtual Player Player { get; set; }
    }
}
