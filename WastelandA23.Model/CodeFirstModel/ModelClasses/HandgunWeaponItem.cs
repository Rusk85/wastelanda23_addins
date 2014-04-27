using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Model.CodeFirstModel
{
    public class HandgunWeaponItem
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public int HandgunWeaponId { get; set; }
        public virtual HandgunWeapon HandgunWeapon { get; set; }
    }
}
