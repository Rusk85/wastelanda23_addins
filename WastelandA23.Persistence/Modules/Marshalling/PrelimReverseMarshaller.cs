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
        }
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

    public class PrelimReverseMarshaller
    {

        private struct TypeCheck
        {
            public bool isScalar;
            public bool isCollection;
            public bool isObject;
            public TypeCheck(bool isScalar,
                bool isCollection,
                bool isObject)
            {
                this.isScalar = isScalar;
                this.isCollection = isCollection;
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
            var collectionProps = tl.Where(_ => _.Item2.isCollection).Select(_ => _.Item1).ToList();
            var objectProps = tl.Where(_ => _.Item2.isObject).Select(_ => _.Item1).ToList();

            scalarProps.ForEach(_ => returnBlock.addElement(marshalFromScalarProperty(_, source)));
            collectionProps.ForEach(_ => returnBlock.addElement(
                new ListBlock(marshalFromScalarPropertyList(_, source).ToList())));
            objectProps.ForEach(_ => returnBlock.addElement(marshalFromObject(_.GetValue(source))));
            return returnBlock;
        }

        private static TypeCheck inspectType<T>(T source)
        {
            bool isType = typeof(T) == typeof(Type);
            bool isCollection = false;
            bool isScalar = false;
            isCollection = typeof(IList).IsAssignableFrom(source as Type);
            isScalar = typeof(string) == source as Type;
            // !false && !true => true && false => false
            // !false && !false => true && true => true
            bool isObject = !isScalar && !isCollection;
            return new TypeCheck(isScalar, isCollection, isObject);
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

    }
}
