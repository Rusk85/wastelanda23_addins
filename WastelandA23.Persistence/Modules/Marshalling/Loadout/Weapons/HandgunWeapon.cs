using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public class HandgunWeapon : AbstractWeapon
    {
        [ParamNumber(0)]
        public List<HandgunItems> handgunItems{ get; set; }

        [ParamNumber(1)]
        public override LoadedMagazine loadedMagazines { get; set; }

        public HandgunWeapon() { }

        public HandgunWeapon(List<HandgunItems> handgunItem, LoadedMagazine loadedMagazines)
        {
            this.handgunItems= handgunItem;
            this.loadedMagazines = loadedMagazines;
        }

    }
}
