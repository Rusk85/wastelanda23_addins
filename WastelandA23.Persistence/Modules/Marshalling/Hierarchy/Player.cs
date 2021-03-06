﻿namespace WastelandA23.Marshalling.Loadout
{
    public class Player
    {
        [ParamNumber(0)]
        public Command Command { get; private set; }

        [ParamNumber(1)]
        public PlayerInfo PlayerInfo { get; private set; }

        [ParamNumber(2)]
        public CurrentWeapon CurrentWeapon { get; private set; }

        [ParamNumber(3)]
        public Loadout Loadout { get; private set; }

        public Player()
        {
        }

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