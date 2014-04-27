using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Model.CodeFirstModel
{
    public class PrimaryWeaponItem
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public int PrimaryWeaponId { get; set; }
        public virtual PrimaryWeapon PrimaryWeapon { get; set; }
    }
}
