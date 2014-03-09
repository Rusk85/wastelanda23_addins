using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    class Player : AbstractMarshalling
    {

        public string PlayerUID { get; private set; }
        public Loadout loadout { get; private set; }
        public CurrentWeapon currentWeapon { get; private set; }
        public CurrentMode currentMode { get; private set; }

        Player(string Command,
               string PlayerUID,
               Loadout loadout,
               CurrentWeapon currentWeapon,
               CurrentMode currentMode) : base(Command){
            
            this.PlayerUID = PlayerUID;
            this.loadout = loadout;
            this.currentWeapon = currentWeapon;
            this.currentMode = currentMode;
        }



    }
}
