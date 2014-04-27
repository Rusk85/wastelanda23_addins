using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Model.CodeFirstModel
{
    public class Backpack
    {
        public int Id { get; set; }

        public string ClassName { get; set; }

        public virtual ICollection<Item> Items { get; set; }

        public virtual Loadout Loadout { get; set; }
    }
}
