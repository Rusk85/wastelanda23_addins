using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Model.CodeFirstModel
{
    public class PlayerInfo
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string UID { get; set; }

        public virtual Player Player { get; set; }
    }
}
