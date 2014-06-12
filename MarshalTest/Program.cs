﻿using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using WastelandA23.Marshalling;
using WastelandA23.Marshalling.Loadout;
using AutoMapper;
using System.Diagnostics;

namespace MarshalTest
{
    public abstract class baseTestObj
    {
        public abstract string baseStr { get; set; }
    }

    public class testObj : baseTestObj
    {
        public testObj()
        {
            strList = new List<string>();
            strList2 = new List<string>() { "1test", "2test" };
            aStr = "aStr";
            innerTestObj = new innerTestClass();
            innerTestObjList = new List<innerTestClass>() { new innerTestClass(), new innerTestClass() };
            baseStr = "baseStr";
            convertMe = new List<int> { 10, 100, 45 };
            time = DateTime.Now;
            imaBool = true;
        }
        [ParamNumber(6)]
        public List<innerTestClass> innerTestObjList { get; set; }
        [ParamNumber(5)]
        public List<string> strList { get; set; }
        public List<string> strList2 { get; set; }
        [ParamNumber(4)]
        public string aStr { get; set; }
        public innerTestClass innerTestObj { get; set; }
        public override string baseStr { get; set; }
        [ParamNumber(2)]
        public List<int> convertMe { get; set; }

        [ParamNumber(1)]
        public DateTime time { get; set; }

        [ParamNumber(3)]
        public bool imaBool { get; set; }

    }

    public class innerTestClass : innerBaseTestClass
    {
        public innerTestClass()
        {
            innerStr = "innerStr";
            innerList = new List<string> { "innerListElem1", "innerListElem2" };
            ignoreStr = "ignoreStr";
        }
        public string innerStr { get; set; }
        public string ignoreStr { get; set; }
        public List<string> innerList { get; set; }
    }

    [DerivedType]
    public class innerBaseTestClass
    {
        public innerBaseTestClass()
        {
            innerBaseInt = 44;
            innerBaseStr = "innerBaseStr";
            dateTime = new List<DateTime> { DateTime.Now, DateTime.Now.AddDays(10) };
        }

        public string innerBaseStr { get; set; }
        public List<DateTime> dateTime { get; set; }
        public int innerBaseInt { get; set; }
    }


    // [[["innerStr","innerStr2"],["innerStr","innerStr2"]]]
    public class testObj2
    {
        public testObj2()
        {
            strList = new List<string>() { "strList1_1", "strList1_2" };
            strList2 = new List<string>() { "strList2_1", "strList2_1" };
            aStr = "aStr";
            aaStr = "aaStr";
            innerTestObj2 = new innerTestObj2();
            innerTestObj = new List<innerTestObj2>() { new innerTestObj2(), new innerTestObj2() };
        }
        public string aaStr { get; set; }
        public List<string> strList { get; set; }
        public string aStr { get; set; }
        public innerTestObj2 innerTestObj2 { get; set; }
        public List<innerTestObj2> innerTestObj { get; set; }
        public List<string> strList2 { get; set; }

    }


    public class innerTestObj2
    {
        public innerTestObj2()
        {
            innerStr = "innerStr";
            innerStr2 = "innerStr2";
        }

        public string innerStr { get; set; }
        public string innerStr2 { get; set; }
    }

    class Program
    {
        //input
        //[""] -> List [null]
        //output
        //null -> 



        //static string test = @"[[""SAVE""],[""Rusk"",""76561197964280320""],[""Colt1911"",[""Colt1911""]],[[""ItemMap"",""ItemCompass"",""ItemWatch"",""H_MilCap_mcamo"",""G_Tactical_Black""],["""",[],[]],[""Colt1911"",["""","""",""""],[""7Rnd_45ACP_1911"",7]],["""",[],[]],[""U_B_CombatUniform_mcam"",[[""LoadedMagazine"",[""7Rnd_45ACP_1911"",7]],""ItemMap""]],[""V_BandollierB_rgr"",[]],[""B_Carryall_cbr"",[]]]]";
        static string test = @"[[""SAVE""],[""Rusk"",""76561197964280320""],[""Colt1911"",[""Colt1911""]],[[[""AssignableItem"",[""ItemMap""]],[""AssignableItem"",[""ItemCompass""]],[""AssignableItem"",[""ItemWatch""]],[""AssignableItem"",[""H_MilCap_mcamo""]],[""AssignableItem"",[""G_Tactical_Black""]]],["""",[],[]],[""Colt1911"",["""","""",""""],[""7Rnd_45ACP_1911"",2]],["""",[],[]],[""U_B_CombatUniform_mcam"",[[""Magazine"",[""7Rnd_45ACP_1911"",7]]]],[""V_BandollierB_rgr"",[]],[""B_Carryall_cbr"",[]]]]";
        static string test_target = @"[[""SAVE""],[""Rusk"",""76561197964280320""],[""srifle_DMR_01_MRCO_pointer_F"",[""Single""]],[[[""AssignableItem"",[""ItemMap""]],[""AssignableItem"",[""ItemCompass""]],[""AssignableItem"",[""ItemWatch""]],[""AssignableItem"",[""H_MilCap_mcamo""]],[""AssignableItem"",[""G_Tactical_Black""]]],[""srifle_DMR_01_MRCO_pointer_F"",["""",""acc_pointer_IR"",""optic_MRCO""],[""10Rnd_762x51_Mag"",10]],[""hgun_Rook40_snds_F"",[""muzzle_snds_L"","""",""""],[""16Rnd_9x21_Mag"",16]],[""launch_O_Titan_short_F"",["""","""",""""],[""Titan_AT"",1]],[""U_B_CombatUniform_mcam"",[[""Magazine"",[""10Rnd_762x51_Mag"",8]],[""Magazine"",[""16Rnd_9x21_Mag"",16]],[""Magazine"",[""16Rnd_9x21_Mag"",16]],[""Magazine"",[""10Rnd_762x51_Mag"",8]]]],[""V_BandollierB_rgr"",[""SmokeShellPurple""]],[""B_Carryall_cbr"",[[""Magazine"",[""Titan_AT"",1]]]]]]";


        static private void DressRehearsal_InputToOutput()
        {

            TypeMapper.Map<Player, WastelandA23.Model.CodeFirstModel.Player>(new Player(), new WastelandA23.Model.CodeFirstModel.Player());


            TypeConverter.SetConverters(new WastelandTypeConversionDictionary());

            string input = test_target;
            // from SQF-String to player class hierarchy
            Player player = Marshaller.unmarshalFrom<Player>(input);
            var typeMap = Mapper.GetAllTypeMaps();
            //var CFItemMag = typeMap.Where(_ => _.SourceType == typeof(WastelandA23.Model.CodeFirstModel.Item)).FirstOrDefault();
            //CFItemMag.IncludeDerivedTypes(typeof(WastelandA23.Model.CodeFirstModel.Magazine), typeof(Magazine));




            //var MapsWithInheritance = typeMap.Where(_ => _.IncludedDerivedTypes.Any());
            WastelandA23.Model.CodeFirstModel.Player db_player = null;
            string output = null;

            // drop and recreate database
            using (var ctx = new WastelandA23.Model.CodeFirstModel.LoadoutContext())
            {
                Database.SetInitializer(new WastelandA23.Model.Init.LoadoutContextSeed(true));
                ctx.Database.Initialize(true);
            }

            // from player class hierarchy to database via AutoMapper and Entity Framework
            using (var ctx = new WastelandA23.Model.CodeFirstModel.LoadoutContext())
            {
                var type_maps = Mapper.GetAllTypeMaps();
                //Mapper.CreateMap<Item, WastelandA23.Model.CodeFirstModel.Item>().Include<Magazine, WastelandA23.Model.CodeFirstModel.Magazine>();
                var model_player = Mapper.Map<WastelandA23.Model.CodeFirstModel.Player>(player);
                ctx.Players.Add(model_player);
                ctx.SaveChanges();
            }
                using (var ctx = new WastelandA23.Model.CodeFirstModel.LoadoutContext())
                {
                    db_player = ctx.Players.Where(_ => _.PlayerInfo.UID == player.PlayerInfo.UID).Include(_ => _.Loadout.AssignedItems).FirstOrDefault();
                    Player outputPlayer = new Player();
                    //Mapper.CreateMap<WastelandA23.Model.CodeFirstModel.Player, Player>();
                    outputPlayer = Mapper.Map(db_player, outputPlayer);
                    output = Marshaller.marshalFromListBlock(Marshaller.marshalFromObject(outputPlayer));
                    Debug.WriteLine(output);
                }
        }


        static void Main(string[] args)
         {


             DressRehearsal_InputToOutput();
            //var s = new List<string> { "test", "test2" };
            //var res = PrelimReverseMarshaller.marshalFrom(s);
            //var res = PrelimReverseMarshaller.marshalFromObject(new testObj());
            Player player = new Player();
            player = Marshaller.unmarshalFrom<Player>(test_target);
            var res2 = Marshaller.marshalFromObject(player);
            var resStr2 = Marshaller.marshalFromListBlock(res2);
            var res = Marshaller.marshalFromObject(new testObj());
            var resStr = Marshaller.marshalFromListBlock(res);
            Debug.WriteLine(resStr);
            Debug.WriteLine(resStr2);


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
