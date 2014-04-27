using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using WastelandA23.Marshalling;
using WastelandA23.Marshalling.Loadout;

namespace MarshalTest
{
    class Program
    {

        //static string test = @"[[""SAVE""],[""Rusk"",""76561197964280320""],[""Colt1911"",[""Colt1911""]],[[""ItemMap"",""ItemCompass"",""ItemWatch"",""H_MilCap_mcamo"",""G_Tactical_Black""],["""",[],[]],[""Colt1911"",["""","""",""""],[""7Rnd_45ACP_1911"",7]],["""",[],[]],[""U_B_CombatUniform_mcam"",[[""Magazine"",[""7Rnd_45ACP_1911"",7]],""ItemMap""]],[""V_BandollierB_rgr"",[]],[""B_Carryall_cbr"",[]]]]";
        static string test = @"[[""SAVE""],[""Rusk"",""76561197964280320""],[""Colt1911"",[""Colt1911""]],[[[""AssignableItem"",[""ItemMap""]],[""AssignableItem"",[""ItemCompass""]],[""AssignableItem"",[""ItemWatch""]],[""AssignableItem"",[""H_MilCap_mcamo""]],[""AssignableItem"",[""G_Tactical_Black""]]],["""",[],[]],[""Colt1911"",["""","""",""""],[""7Rnd_45ACP_1911"",2]],["""",[],[]],[""U_B_CombatUniform_mcam"",[[""Magazine"",[""7Rnd_45ACP_1911"",7]]]],[""V_BandollierB_rgr"",[]],[""B_Carryall_cbr"",[]]]]";

        static void Main(string[] args)
         {
            Player player = new Player();
            player = Marshaller.unmarshalFrom<Player>(test);


            var i = 0;

            while (i < 1)
            {
                using (var ctx = new WastelandA23.Model.CodeFirstModel.LoadoutContext())
                {
                    Database.SetInitializer(new WastelandA23.Model.Init.LoadoutContextSeed());
                    ctx.Database.Initialize(true);
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
