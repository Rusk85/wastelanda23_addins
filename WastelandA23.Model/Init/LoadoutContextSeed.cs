using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using WastelandA23.Model.CodeFirstModel;

namespace WastelandA23.Model.Init
{
    public class LoadoutContextSeed : DropCreateDatabaseAlways<LoadoutContext>
    {

        private bool drop;

        public LoadoutContextSeed(bool drop = true) 
        {
            this.drop = drop;
        }

        public override void InitializeDatabase(LoadoutContext context)
        {
            if (drop) { base.InitializeDatabase(context); }
        }

        protected override void Seed(LoadoutContext context)
        {
            return;
            var playerInfo = new PlayerInfo() { Name = "Rusk", UID = "100" };
            var currentWpn = new CurrentWeapon() { ClassName = "CurrWpn", CurrentMode = new CurrentMode() { Mode = "FullAuto" } };
            var backpack = new Backpack() { ClassName = "Backpack", Items = new List<Item> 
                { new Item() { ClassName = "Backpack_Item1" }, new Item() { ClassName = "Backpack_Item2" } } };
            var vest = new Vest()
            {
                ClassName = "Vest",
                Items = new List<Item> { new Item() { ClassName = "Vest_Item1" }, new Item() { ClassName = "Vest_Item2" } }
            };

            var uniform = new Uniform() 
            {
                ClassName="Uniform", 
                Items= new List<Item> { new Item() { ClassName = "Uniform_Item1" }, new Item() { ClassName = "Uniform_Item2" } }
            };

            var pWpn = new PrimaryWeapon()
            {
                ClassName = "PrimaryWeapon",
                LoadedMagazine = new Magazine() { ClassName = "PrimWpnMag", Bullets = 100 },
                PrimaryWeaponItems = new List<PrimaryWeaponItem> { new PrimaryWeaponItem() { ClassName = "PrimWpnItem_Silencer" } }
            };

            var sWpn = new SecondaryWeapon()
            {
                ClassName = "SecondaryWeapon",
                LoadedMagazine = new Magazine() { ClassName = "SecWpnMag", Bullets = 22 },
                SecondaryWeaponItems = new List<SecondaryWeaponItem>
                {
                    new SecondaryWeaponItem(){ClassName="SecWpnItem_Scope"}
                }
            };

            var hWpn = new HandgunWeapon()
            {
                ClassName = "HandgunWeapon",
                LoadedMagazine = new Magazine() { ClassName = "handWpnMag", Bullets = 10 },
                HandgunWeaponItems = new List<HandgunWeaponItem>
                {
                    new HandgunWeaponItem(){ClassName="HandGunWeaponItem_TacFlash"}
                }
            };

            var loadout = new Loadout()
            {
                AssignedItems = new List<AssignableItem> { new AssignableItem() { ClassName = "AssignableItem_Compass" } },
                Backpack = backpack,
                Uniform = uniform,
                Vest = vest,
                HandgunWeapon = hWpn,
                PrimaryWeapon = pWpn,
                SecondaryWeapon = sWpn
            };

            var player = new Player() { PlayerInfo = playerInfo, CurrentWeapon = currentWpn, Loadout = loadout };

            context.Players.Add(player);
            context.SaveChanges();
        }


        



    }
}
