using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public class CurrentWeapon
    {
        public AbstractWeapon currentWeapon { get; private set; }

        public CurrentWeapon(AbstractWeapon currentWeapon)
        {
            this.currentWeapon = currentWeapon;
        }

    }
}
