using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Model.CodeFirstModel
{
    public class CurrentMode
    {
        public int Id { get; set; }
        public string Mode { get; set; }
        public virtual CurrentWeapon CurrentWeapon { get; set; }
    }
}
