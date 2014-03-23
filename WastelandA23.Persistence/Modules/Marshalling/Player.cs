using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public class Player
    {
        public class Request
        {
            public string Command { get; set; }
            public string PlayerUID { get; set; }
        }

        [ParamNumber(0)]
        public Request sqlRequest { get; private set; }

        [ParamNumber(1)]
        public Loadout loadout { get; private set; }
        public CurrentWeapon currentWeapon { get; private set; }
        public CurrentMode currentMode { get; private set; }
        
        public Player() { }

        /*
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
        */
    }
}
