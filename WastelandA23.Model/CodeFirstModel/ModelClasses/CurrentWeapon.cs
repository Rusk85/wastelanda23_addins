using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Model.CodeFirstModel
{
    public class CurrentWeapon
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public virtual CurrentMode CurrentMode { get; set; }
        public virtual Player Player { get; set; }
    }
}
