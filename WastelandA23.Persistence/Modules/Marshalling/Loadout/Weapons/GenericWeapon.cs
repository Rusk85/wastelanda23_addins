using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    /// <summary>
    /// When assigning a weapon to current weapon we can't now of what (Arma3) type it is.
    /// </summary>
    public class GenericWeapon : AbstractWeapon
    {
        public override LoadedMagazine loadedMagazines { get; set; }

        public GenericWeapon() { }
    }
}
