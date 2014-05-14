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
            error = -1;
        }
        [ParamNumber(1)]
        public List<innerTestObj> innerTestObjList { get; set; }
        [ParamNumber(2)]
        public int error { get; set; }
        public List<string> strList { get; set; }
        public List<string> strList2 {get; set;}
        [ParamNumber(0)]
        public string aStr {get; set;}
        public innerTestObj innerTestObj { get; set; }
    }

    public class innerTestObj
    {
        public innerTestObj()
        {
            innerStr = "innerStr";
            ignoreStr = "ignoreMe";
            innerList = new List<string> { "innerListElement#1", "innerListElement#2" };
        }
        [ParamNumber(1)]
        public string innerStr { get; set; }
        public string ignoreStr { get; set; }
        [ParamNumber(0)]
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

        private struct PropertyOrder
        {
            public PropertyInfo PropertyInfo;
            public TypeCheck TypeCheck;
            public ParamNumberAttribute ParamNumber;
            public int? Order;
            public PropertyOrder
            (
                PropertyInfo PropertyInfo,
                TypeCheck TypeCheck,
                ParamNumberAttribute ParamNumber,
                int? Order
            )
            {
                this.PropertyInfo = PropertyInfo;
                this.TypeCheck = TypeCheck;
                this.ParamNumber = ParamNumber;
                this.Order = Order;
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

            Func<List<Tuple<PropertyInfo, TypeCheck>>, List<PropertyOrder>> sortListByProp = 
                delegate(List<Tuple<PropertyInfo, TypeCheck>> tpl)
            {
                var attrList = tpl.Where(_ => _.Item1.GetCustomAttribute<ParamNumberAttribute>() != null).ToList();
                var retList = new List<PropertyOrder>();
                if (attrList.Count > 0)
                {
                    attrList.ForEach(_ => retList.Add(new PropertyOrder
                        (
                        _.Item1,
                        _.Item2,
                        _.Item1.GetCustomAttribute<ParamNumberAttribute>(),
                        _.Item1.GetCustomAttribute<ParamNumberAttribute>().parameterIndex
                        )));
                    retList = retList.OrderBy(_ => _.Order).ToList();
                }
                else
                {
                    tpl.ForEach(_ => retList.Add(new PropertyOrder(_.Item1, _.Item2, null, null)));
                }
                return retList;
            };

            var tl = new List<Tuple<PropertyInfo, TypeCheck>>();
            props.ForEach(_ => tl.Add(typeCheck(_)));
            var orderedProps = sortListByProp(tl);

            foreach (var tpl in orderedProps)
            {
                var prop = tpl.PropertyInfo;
                var type = tpl.TypeCheck;

                if (type.isScalar)
                {
                    returnBlock.addElement(marshalFromScalarProperty(prop, source));
                }
                else if (type.isScalarCollection)
                {
                    returnBlock.addElement(new ListBlock(
                        marshalFromScalarPropertyList(prop, source).ToList()));
                }
                else if (type.isObject)
                {
                    returnBlock.addElement(marshalFromObject(prop.GetValue(source)));
                }
                else if (type.isObjectCollection)
                {
                    Type elementType = prop.PropertyType.GetGenericArguments()[0];
                    returnBlock.addElement(new ListBlock(marshalFromObjectPropertyList(
                        prop, source, Activator.CreateInstance(elementType)).ToList()));
                }
            }
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
            if (!isScalar && (source as Type).IsPrimitive)
            {
                throw new NotImplementedException("Marshalling for Type" + (source as Type).FullName + " not supported.");
            }
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
