using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Model.CodeFirstModel
{
    public class Player
    {
        public int Id { get; set; }

        public virtual PlayerInfo PlayerInfo { get; set; }

        public virtual CurrentWeapon CurrentWeapon { get; set; }

        public virtual Loadout Loadout { get; set; }
    }
}
