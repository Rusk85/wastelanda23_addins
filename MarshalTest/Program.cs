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

        static string test = @"[[""SAVE"",""76561197964280320""],[""Colt1911"",[""Colt1911""]],[[""ItemMap"",""ItemCompass"",""ItemWatch"",""H_MilCap_mcamo"",""G_Tactical_Black""],["""",[],[]],[""Colt1911"",["""","""",""""],[""7Rnd_45ACP_1911"",7]],["""",[],[]],[""U_B_CombatUniform_mcam"",[[""Magazine"",[""7Rnd_45ACP_1911"",7]],""ItemMap""]],[""V_BandollierB_rgr"",[]],[""B_Carryall_cbr"",[]]]]";

        static private void testMethod<T>(T obj, Test t)
        {
            Type type = typeof(T);
            Console.WriteLine(type.Name);
        }

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
