using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Model.CodeFirstModel
{
    public class Item
    {
        public int Id { get; set; }
        public string ClassName { get; set; }

        public int? BackpackId { get; set; }

        public int? UniformId { get; set; }

        public int? VestId { get; set; }

        public virtual Backpack Backpack { get; set; }

        public virtual Uniform Uniform { get; set; }

        public virtual Vest Vest { get; set; }
    }
}
