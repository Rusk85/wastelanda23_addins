using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using WastelandA23.Marshalling;
using WastelandA23.Marshalling.Loadout;
using AutoMapper;

namespace MarshalTest
{
    class Program
    {

        //static string test = @"[[""SAVE""],[""Rusk"",""76561197964280320""],[""Colt1911"",[""Colt1911""]],[[""ItemMap"",""ItemCompass"",""ItemWatch"",""H_MilCap_mcamo"",""G_Tactical_Black""],["""",[],[]],[""Colt1911"",["""","""",""""],[""7Rnd_45ACP_1911"",7]],["""",[],[]],[""U_B_CombatUniform_mcam"",[[""LoadedMagazine"",[""7Rnd_45ACP_1911"",7]],""ItemMap""]],[""V_BandollierB_rgr"",[]],[""B_Carryall_cbr"",[]]]]";
        static string test = @"[[""SAVE""],[""Rusk"",""76561197964280320""],[""Colt1911"",[""Colt1911""]],[[[""AssignableItem"",[""ItemMap""]],[""AssignableItem"",[""ItemCompass""]],[""AssignableItem"",[""ItemWatch""]],[""AssignableItem"",[""H_MilCap_mcamo""]],[""AssignableItem"",[""G_Tactical_Black""]]],["""",[],[]],[""Colt1911"",["""","""",""""],[""7Rnd_45ACP_1911"",2]],["""",[],[]],[""U_B_CombatUniform_mcam"",[[""Magazine"",[""7Rnd_45ACP_1911"",7]]]],[""V_BandollierB_rgr"",[]],[""B_Carryall_cbr"",[]]]]";

        static string test_target = @"[[""SAVE""],[""Rusk"",""76561197964280320""],[""srifle_DMR_01_MRCO_pointer_F"",[""Single""]],[[[""AssignableItem"",[""ItemMap""]],[""AssignableItem"",[""ItemCompass""]],[""AssignableItem"",[""ItemWatch""]],[""AssignableItem"",[""H_MilCap_mcamo""]],[""AssignableItem"",[""G_Tactical_Black""]]],[""srifle_DMR_01_MRCO_pointer_F"",["""",""acc_pointer_IR"",""optic_MRCO""],[""10Rnd_762x51_Mag"",10]],[""hgun_Rook40_snds_F"",[""muzzle_snds_L"","""",""""],[""16Rnd_9x21_Mag"",16]],[""launch_O_Titan_short_F"",["""","""",""""],[""Titan_AT"",1]],[""U_B_CombatUniform_mcam"",[[""Magazine"",[""10Rnd_762x51_Mag"",8]],[""Magazine"",[""16Rnd_9x21_Mag"",16]],[""Magazine"",[""16Rnd_9x21_Mag"",16]],[""Magazine"",[""10Rnd_762x51_Mag"",8]]]],[""V_BandollierB_rgr"",[""SmokeShellPurple""]],[""B_Carryall_cbr"",[[""Magazine"",[""Titan_AT"",1]]]]]]";

        static void Main(string[] args)
         {


            var i = 0;

            while (i < 1)
            {
                using (var ctx = new WastelandA23.Model.CodeFirstModel.LoadoutContext())
                {
                    Database.SetInitializer(new WastelandA23.Model.Init.LoadoutContextSeed(true));
                    ctx.Database.Initialize(true);
                }

                using (var ctx = new WastelandA23.Model.CodeFirstModel.LoadoutContext())
                {
                    Player player = new Player();
                    player = Marshaller.unmarshalFrom<Player>(test_target);
                    var type_maps = Mapper.GetAllTypeMaps();
                    //Mapper.CreateMap<Item, WastelandA23.Model.CodeFirstModel.Item>().Include<Magazine, WastelandA23.Model.CodeFirstModel.Magazine>();
                    var model_player = Mapper.Map<WastelandA23.Model.CodeFirstModel.Player>(player);
                    ctx.Players.Add(model_player);
                    ctx.SaveChanges();


                }

                using (var ctx = new WastelandA23.Model.CodeFirstModel.LoadoutContext())
                {
                    var backpack = ctx.Backpacks.First();
                    ctx.Backpacks.Remove(backpack);
                    ctx.SaveChanges();
                    
                    var p = ctx.Players.FirstOrDefault();
                    ctx.Players.Remove(p);
                    ctx.SaveChanges();
                }


                i++;
            }






            Console.ReadLine();
        }
    }
}
