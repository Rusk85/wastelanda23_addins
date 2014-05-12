using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;

namespace WastelandA23.Marshalling
{
    public class testObj
    {
        public testObj()
        {
            strList = new List<string>() { "test1", "test2" };
            strList2 = new List<string>() { "1test", "2test" };
            aStr = "aStr";
            innerTestObj = new innerTestObj();
            innerTestObjList = new List<innerTestObj>() { new innerTestObj(), new innerTestObj() };
        }
        public List<innerTestObj> innerTestObjList { get; set; }
        public List<string> strList { get; set; }
        public List<string> strList2 {get; set;}
        public string aStr {get; set;}
        public innerTestObj innerTestObj { get; set; }
    }

    public class innerTestObj
    {
        public innerTestObj()
        {
            innerStr = "innerStr";
            innerList = new List<string> { "innerListElement#1", "innerListElement#2" };
        }
        public string innerStr { get; set; }
        public List<string> innerList { get; set; }
    }


    // [[["innerStr","innerStr2"],["innerStr","innerStr2"]]]
    public class testObj2
    {
        public testObj2()
        {
            innerTestObj = new List<innerTestObj2>() { new innerTestObj2(), new innerTestObj2() };
        }
        public List<innerTestObj2> innerTestObj { get; set; }
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


    public class PrelimReverseMarshaller
    {

        private struct TypeCheck
        {
            public bool isScalar;
            public bool isScalarCollection;
            public bool isObjectCollection;
            public bool isObject;
            public TypeCheck
            (
                bool isScalar,
                bool isScalarCollection,
                bool isObjectCollection,
                bool isObject
            )
            {
                this.isScalar = isScalar;
                this.isScalarCollection = isScalarCollection;
                this.isObjectCollection = isObjectCollection;
                this.isObject = isObject;
            }
        }

        public static ListBlock marshalFromObject<T>(T source)
        {
            var returnBlock = new ListBlock();
            PropertyInfo[] props = source.GetType().GetProperties();
            Func<PropertyInfo, Tuple<PropertyInfo, TypeCheck>> typeCheck = delegate(PropertyInfo pi)
            {
                return Tuple.Create(pi, inspectType(pi.PropertyType));
            };
            var tl = new List<Tuple<PropertyInfo, TypeCheck>>();

            props.ForEach(_ => tl.Add(typeCheck(_)));

            var scalarProps = tl.Where(_ => _.Item2.isScalar).Select(_ => _.Item1).ToList();
            var scalarCollectionProps = tl.Where(_ => _.Item2.isScalarCollection).Select(_ => _.Item1).ToList();
            var objectCollectionProps = tl.Where(_ => _.Item2.isObjectCollection).Select(_ => _.Item1).ToList();
            var objectProps = tl.Where(_ => _.Item2.isObject).Select(_ => _.Item1).ToList();

            scalarProps.ForEach(_ => returnBlock.addElement(marshalFromScalarProperty(_, source)));

            scalarCollectionProps.ForEach(_ => returnBlock.addElement(
                new ListBlock(marshalFromScalarPropertyList(_, source).ToList())));

            objectCollectionProps.ForEach(_ => returnBlock.addElement(
                new ListBlock(marshalFromObjectPropertyList(_, source, 
                    Activator.CreateInstance(_.PropertyType.GetGenericArguments()[0])).ToList())));

            objectProps.ForEach(_ => returnBlock.addElement(marshalFromObject(_.GetValue(source))));

            return returnBlock;
        }

        private static TypeCheck inspectType<T>(T source)
        {
            bool isScalar = false;
            bool isScalarCollection = false;
            bool isObjectCollection = false;

            if (typeof(IList).IsAssignableFrom(source as Type))
            {
                Type elementType = (source as Type).GetGenericArguments()[0];
                isScalarCollection = elementType == typeof(string);
                isObjectCollection = !isScalarCollection;
            }
            isScalar = typeof(string) == source as Type;
            bool isObject = !isScalar && !isScalarCollection && !isObjectCollection;

            return new TypeCheck(isScalar, isScalarCollection, isObjectCollection, isObject);
        }

        private static ListBlock marshalFromScalarProperty<T>
            (PropertyInfo PropertyInfo, T source)
        {
            return new ListBlock(PropertyInfo.GetValue(source) as string);
        }

        private static IList<ListBlock> marshalFromScalarPropertyList<T>
            (PropertyInfo PropertyInfo, T source)
        {
            var ret = new List<ListBlock>();
            var srcList = PropertyInfo.GetValue(source) as List<string>;
            srcList.ForEach(_ => ret.Add(new ListBlock(_)));
            return ret;
        }

        private static IList<ListBlock> marshalFromObjectPropertyList<T, Te>
            (PropertyInfo PropertyInfo, T source, Te elemType)
        {
            var ret = new List<ListBlock>();
            var srcList = (PropertyInfo.GetValue(source) as IList).Cast<Te>().ToList();
            srcList.ForEach(_ => ret.Add(marshalFromObject(_)));
            return ret;
        }


    }
}
