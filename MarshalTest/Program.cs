using System;
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
        class Test2 {
            public string a1, a2;
        }
        class Test
        {
            [ParamNumber(0)]
            public string v1;
            [ParamNumber(1)]
            public string v2;
        }

        //static string test = @"[[""SAVE_COMMAND"",""76561197964280320""],[[""ItemMap"",""ItemCompass"",""ItemWatch"",""Binocular_Vector"",""H_MilCap_mcamo"",""G_Tactical_Black""],[""srifle_EBR_ARCO_pointer_snds_F"",[""muzzle_snds_B"",""acc_pointer_IR"",""optic_Arco""]],[""hgun_Rook40_snds_F"",[""muzzle_snds_L"","""",""""]],[""launch_Titan_F"",["""","""",""""]],[""U_B_CombatUniform_mcam"",[""muzzle_snds_B"",""acc_pointer_IR"",""optic_Arco"",""30Rnd_556x45_Stanag"",""30Rnd_65x39_caseless_green""]],[""V_BandollierB_rgr"",[""Colt1911"",""8Rnd_9x18_MakarovSD"",""8Rnd_9x18_MakarovSD"",""8Rnd_9x18_MakarovSD"",""8Rnd_9x18_MakarovSD"",""8Rnd_9x18_MakarovSD"",""8Rnd_9x18_MakarovSD"",""8Rnd_9x18_Makarov"",""8Rnd_9x18_Makarov"",""8Rnd_9x18_Makarov"",""16Rnd_9x21_Mag"",""16Rnd_9x21_Mag"",""16Rnd_9x21_Mag"",""16Rnd_9x21_Mag"",""30Rnd_65x39_caseless_green""]],[""B_Carryall_cbr"",[""optic_LRPS"",""M16A2"",""arifle_TRG21_GL_ACO_pointer_F"",""15Rnd_9x19_M9SD"",""15Rnd_9x19_M9SD"",""15Rnd_9x19_M9SD"",""8Rnd_9x18_Makarov"",""30Rnd_556x45_Stanag"",""30Rnd_556x45_Stanag"",""75Rnd_545x39_RPK"",""30Rnd_545x39_AKSD"",""30Rnd_545x39_AK"",""7Rnd_45ACP_1911"",""30Rnd_9x19_MP5"",""FlareRed_GP25"",""FlareGreen_GP25"",""1Rnd_HE_GP25"",""1Rnd_HE_GP25"",""HandGrenadeTimed"",""30Rnd_65x39_caseless_green"",""20Rnd_762x51_Mag"",""20Rnd_762x51_Mag"",""20Rnd_762x51_Mag""]],[[""20Rnd_762x51_Mag""],[""16Rnd_9x21_Mag""],[""Titan_AA""],[""Titan_AA"",""Titan_AA"",""Titan_AA""]],"""",""""]]";
        //static string test = @"[[""SAVE_COMMAND"",""76561197964280320""],[[""ItemMap"",""ItemCompass"",""ItemWatch"",""Binocular_Vector""],[""arifle_MX_RCO_pointer_snds_F"",[""muzzle_snds_H"",""acc_pointer_IR"",""optic_Arco""]],[""Colt1911"",["""","""",""""]],[""launch_Titan_short_F"",["""","""",""""]],[""U_B_CombatUniform_mcam"",[""7Rnd_45ACP_1911""]],[""V_BandollierB_rgr"",[""8Rnd_9x18_MakarovSD""]],[""B_Carryall_cbr"",[""30Rnd_545x39_AK""]]]]";
        //static string test = @"[[""SAVE_COMMAND"",""76561197964280320""],[[""ItemMap"",""ItemCompass"",""ItemWatch"",""H_MilCap_mcamo"",""G_Tactical_Black""],[""arifle_Katiba_ARCO_F"",["""","""",""optic_Arco""]],[""Colt1911"",["""","""",""""]],[""launch_I_Titan_short_F"",["""","""",""""]],[""U_B_CombatUniform_mcam"",[""7Rnd_45ACP_1911"",""30Rnd_65x39_caseless_green""]],[""V_BandollierB_rgr"",[]],[""B_Carryall_cbr"",[""Titan_AT""]],[[""30Rnd_65x39_caseless_green"",13],[""7Rnd_45ACP_1911"",7],[""Titan_AT"",1]],[""arifle_Katiba_ARCO_F"",[""Single""]]]]";
        static string test = @"[[""SAVE"",""76561197964280320""],[""AKS_GOLD"",[""FullAuto""]],[[""ItemMap"",""ItemCompass"",""ItemWatch"",""H_MilCap_mcamo"",""G_Tactical_Black""],[""AKS_GOLD"",["""","""",""""],[""30Rnd_762x39_AK47"",23]],[""Colt1911"",["""","""",""""],[""7Rnd_45ACP_1911"",7]],[""launch_B_Titan_short_F"",["""","""",""""],[""Titan_AT"",1]],[""U_B_CombatUniform_mcam"",[]],[""V_BandollierB_rgr"",[]],[""B_Carryall_cbr"",[]]]]";

        static private void testMethod<T>(T obj, Test t)
        {
            Type type = typeof(T);
            Console.WriteLine(type.Name);
        }

        /*
        static IList<T> testM<T>(IList<Marshaller.ListBlock> a) where T: class
        {
            Console.WriteLine("list bla func");
            return (IList<T>)Activator.CreateInstance(typeof(IList<T>));
        }

        static T testM<T>(IList<Marshaller.ListBlock> a) where T: class
        {
            Console.WriteLine("scalar bla func");
            return (T)Activator.CreateInstance(typeof(T));
        }
        */

        static void Main(string[] args)
         {
            Test obj = null;
            Player player = new Player();
            //Marshaller.ListBlock block = Marshaller.explodeNested(test);
            //obj = Marshaller.marshalFrom(obj, "[[1,2]]");

            player = Marshaller.unmarshalFrom<Player>(test);
            var dict = Marshaller.createParamNumberDictionaryWithInheritance(typeof(PrimaryWeapon));

            Console.ReadLine();
            
            /* 
            Type type = typeof(Test);
            Object val = obj;
            var castMethod = typeof(Program).GetMethod("testMethod", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(type);
            castMethod.Invoke(new Marshaller(), new object[] { val, val });
             */

            Console.ReadLine();
        }
    }
}
