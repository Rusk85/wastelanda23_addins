using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WastelandA23.Marshalling.Loadout;

namespace WastelandA23.Marshalling
{
    public interface IConversionDictionary
    {
        IDictionary<Type, Tuple<Func<Object, string>, Func<string, Object>>> GetConversionDictionary();
    }

    public class DefaultTypeConversionDictionary : IConversionDictionary
    {

        protected IDictionary<Type, Tuple<Func<Object, string>, Func<string, Object>>> ConversionDictionary;

        public DefaultTypeConversionDictionary()
        {
            ConversionDictionary = new Dictionary<Type, Tuple<Func<Object, string>, Func<string, Object>>>();
            SetDefaultConverters();
        }


        public IDictionary<Type, Tuple<Func<object, string>, Func<string, object>>> GetConversionDictionary()
        {
            return ConversionDictionary;
        }

        private void SetDefaultConverters()
        {
            Func<Object, string> outCon;
            Func<string, Object> inCon;
            Func<Func<Object, string>, Func<string, Object>,
                Tuple<Func<Object, string>, Func<string, Object>>> New =
                delegate(Func<Object, string> oCon, Func<string, Object> iCon)
                {
                    return Tuple.Create(oCon, iCon);
                };

            // universal output converter function
            outCon = i => Convert.ToString(i);

            // int
            var t = typeof(Int32);
            inCon = i => Convert.ToInt32(i);
            ConversionDictionary.Add(t, New(outCon, inCon));

            // datetime
            t = typeof(DateTime);
            inCon = i => Convert.ToDateTime(i);
            ConversionDictionary.Add(t, New(outCon, inCon));

            // bool
            t = typeof(bool);
            inCon = _ => Convert.ToBoolean(_);
            ConversionDictionary.Add(t, New(outCon, inCon));

            // string
            t = typeof(string);
            inCon = _ => _;
            ConversionDictionary.Add(t, New(outCon, inCon));
        }

    }

    public sealed class WastelandTypeConversionDictionary : DefaultTypeConversionDictionary
    {

        public WastelandTypeConversionDictionary() : base()
        {
            SetWastelandTypeConverters();
        }

        public IDictionary<Type, Tuple<Func<object, string>, Func<string, object>>> GetConversionDictionary()
        {
            return base.GetConversionDictionary();
        }


        private void SetWastelandTypeConverters()
        {
            Func<Object, string> outCon;
            Func<string, Object> inCon;
            Func<Func<Object, string>, Func<string, Object>,
                Tuple<Func<Object, string>, Func<string, Object>>> New =
                delegate(Func<Object, string> oCon, Func<string, Object> iCon)
                {
                    return Tuple.Create(oCon, iCon);
                };

            Type handgunItem = typeof(HandgunWeaponItem);
            outCon = _ => (_ as HandgunWeaponItem).ClassName;
            inCon = _ => new HandgunWeaponItem { ClassName = _ };
            ConversionDictionary.Add(handgunItem, New(outCon, inCon));

            Type primaryWpnItem = typeof(PrimaryWeaponItem);
            outCon = _ => (_ as PrimaryWeaponItem).ClassName;
            inCon = _ => new PrimaryWeaponItem { ClassName = _ };
            ConversionDictionary.Add(primaryWpnItem, New(outCon, inCon));

            Type secondaryWpnItem = typeof(SecondaryWeaponItem);
            outCon = _ => (_ as SecondaryWeaponItem).ClassName;
            inCon = _ => new SecondaryWeaponItem { ClassName = _ };
            ConversionDictionary.Add(secondaryWpnItem, New(outCon, inCon));

        }

    }

    public static class TypeConverter
    {

        private static IDictionary<Type, Tuple<Func<Object, string>, Func<string, Object>>> ConversionDictionary;

        static TypeConverter()
        {
            ConversionDictionary =
                new Dictionary
                    <
                        Type,
                        Tuple<Func<Object, string>,
                        Func<string, Object>
                    >>();
        }

        public static void SetConverters(IConversionDictionary ConversionDictionary)
        {
            TypeConverter.ConversionDictionary = ConversionDictionary.GetConversionDictionary();
        }


        public static Func<string, Object> GetInputConverter(Type inputType)
        {
            Tuple<Func<Object, string>, Func<string, Object>> retVal;
            if (ConversionDictionary.TryGetValue(inputType, out retVal))
            {
                if (retVal.Item2 != null)
                {
                    return retVal.Item2;
                }
            }
            return null;
        }

        public static Func<Object, string> GetOutputConverter(Type outputType)
        {
            Tuple<Func<Object, string>, Func<string, Object>> retVal;
            if (ConversionDictionary.TryGetValue(outputType, out retVal))
            {
                if (retVal.Item1 != null)
                {
                    return retVal.Item1;
                }
            }
            return null;
        }

        public static void AddConverter(Type Type,
            Func<string, Object> InputConverter,
            Func<Object, string> OutputConverter)
        {
            if (!ConversionDictionary.Keys.Contains(Type))
            {
                ConversionDictionary.Add(Type, Tuple.Create(OutputConverter, InputConverter));
            }
        }
    }
    
}
