using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Model.CodeFirstModel
{
    public class SecondaryWeapon
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public virtual Magazine Magazine { get; set; }
        public virtual Loadout Loadout { get; set; }
        public virtual ICollection<SecondaryWeaponItem> SecondaryWeaponItems { get; set; }
    }
}
