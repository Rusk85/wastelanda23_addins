using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Model.CodeFirstModel
{
    public class AssignableItem
    {
        public int Id { get; set; }
        
        public string ClassName { get; set; }
        
        public int? LoadoutId { get; set; }

        public virtual Loadout Loadout { get; set; }
    }
}
