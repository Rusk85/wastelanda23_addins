using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Data.Entity;
using MoreLinq;
using AutoMapper;

namespace WastelandA23.Marshalling
{
    public class ListBlock
    {
        public ListBlock()
        {
        }

        public ListBlock(string value)
        {
            if (value == null)
            {
                isNullStringValue = true;
            }
            this.value = value;
        }

        public ListBlock(List<ListBlock> block)
        {
            this.block = block;
        }

        public void addElement(ListBlock block)
        {
            if (this.block == null)
            {
                this.block = new List<ListBlock>();
            }
            this.block.Add(block);
        }

        public bool isEmpty()
        {
            return !isValue() && !isArray();
        }

        public bool isValue()
        {
            return isNullStringValue || value != null;
        }

        public bool isArray()
        {
            return block != null;
        }

        public string value = null;
        public List<ListBlock> block = null;

        private bool isNullStringValue = false;
    }

    public class Marshaller
    {
        public static Assembly ModelAssembly = Assembly.GetAssembly(typeof(WastelandA23.Model.CodeFirstModel.LoadoutContext));
        //static string test = "[[SAVE_COMMAND,76561197964280320],[1.2, 3.4]]";

        private static List<string> explodeIfNotEscaped(string str, char blockStart, char blockEnd, char delimiter)
        {
            List<string> result = new List<string>();

            if (isEmptySQFString(str))
            {
                return result;
            }

            int start = 0;
            Boolean ignore = false;

            for (int i = 0; i < str.Length; ++i)
            {
                char current = str[i];

                if (current == blockStart)
                {
                    ignore = true;
                }
                if (current == blockEnd)
                {
                    ignore = false;
                }

                if ((current == delimiter || i == str.Length - 1) && !ignore)
                {
                    var end = i;
                    if (current == delimiter)
                    {
                        end--;
                    }
                    string sub = str.Substring(start, end - start + 1);

                    if (!isEmptySQFString(sub))
                    {
                        result.Add(sub);
                    }
                    start = i + 1;
                }
            }
            return result;
        }

        private static string trimOnce(string str, string beginString, string endString)
        {
            if (String.IsNullOrEmpty(str))
            {
                return str;
            }

            if (str.StartsWith(beginString) && str.EndsWith(endString))
            {
                return str.Substring(beginString.Length, str.Length - 1 - endString.Length);
            }
            return str;
        }

        static public bool isEmptySQFString(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return true;
            }
            if (str.StartsWith("\"") && str.EndsWith("\"") && str.Length == 2)
            {
                return true;
            }
            return false;
        }

        static public ListBlock explodeNested(string str)
        {
            ListBlock result = new ListBlock();

            if (isEmptySQFString(str))
            {
                return result;
            }

            str = trimOnce(str, "[", "]");

            if (!str.StartsWith("[") && !str.EndsWith("]"))
            {
                var valueList = explodeIfNotEscaped(str, '\"', '\"', ',');
                var list = valueList.Select(f => new ListBlock(f.Trim('\"'))).ToList();
                if (list.Count > 0) { result.block = list; };
                return result;
            }

            int braceBalanced = 0;
            int start = 0;
            int end = 0;

            for (int i = start; i < str.Length; ++i)
            {
                if (str[i] == '[')
                {
                    braceBalanced++;
                }

                if (str[i] == ']')
                {
                    braceBalanced--;
                }

                if (braceBalanced == 0 && (str[i] == ',' || i == str.Length - 1))
                {
                    end = str.Length - 1;
                    if (str[i] == ',')
                    {
                        end = i - 1;
                    }

                    string sub = str.Substring(start, end - start + 1);

                    if (sub.StartsWith("[") && sub.EndsWith("]"))
                    {
                        result.addElement(explodeNested(sub));
                    }
                    else if (!isEmptySQFString(str))
                    {
                        sub = sub.Trim('\"');
                        result.addElement(new ListBlock(sub));
                    }
                    start = i + 1;
                }
            }
            return result;
        }

        static private object dynamicCall(String name, Type genericArg, object[] parameters)
        {
            var dynamicMethod = typeof(Marshaller).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).MakeGenericMethod(genericArg);
            return dynamicMethod.Invoke(null, parameters);
        }

        static private IDictionary<int, Tuple<MemberInfo, Func<string, Object>, Func<Object, string>>> createParamNumberDictionaryWithInheritance(Type type, int paramNumberOffset)
        {
            Type baseType = type.BaseType;

            if (type == null || type == typeof(Object))
            {
                return new Dictionary<int, Tuple<MemberInfo, Func<string, Object>, Func<Object, string>>>();
            }

            if (baseType == typeof(Object))
            {
                return createParamNumberDictionary(type, paramNumberOffset);
            }
            else
            {
                var dictBase = createParamNumberDictionaryWithInheritance(baseType, paramNumberOffset);
                var dictThis = createParamNumberDictionary(type, paramNumberOffset + dictBase.Count);
                return dictBase.Union(dictThis).ToDictionary(k => k.Key, v => v.Value);
            }
        }

        static public IDictionary<int, Tuple<MemberInfo, Func<string, Object>, Func<Object, string>>> createParamNumberDictionaryWithInheritance(Type type)
        {
            return createParamNumberDictionaryWithInheritance(type, 0);
        }

        static public IDictionary<int, Tuple<MemberInfo, Func<string, Object>, Func<Object, string>>> createParamNumberDictionary(Type type, int indexOffset = 0)
        {
            Func<MemberInfo, bool> hasParamAttribute = ((m) => m.GetCustomAttribute(typeof(ParamNumberAttribute)) != null);
            Func<MemberInfo, bool> hasIgnoreMemberAttribute = ((m) => m.GetCustomAttribute(typeof(IgnoredMemberAttribute)) != null);
            MemberFilter isNonAbstract = delegate(MemberInfo m, object filter)
            {
                if (m is PropertyInfo)
                {
                    return !((PropertyInfo)m).GetMethod.IsAbstract;
                }
                return true;
            };

            var fields = type.FindMembers(MemberTypes.Property | MemberTypes.Field,
                                          BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly,
                                          isNonAbstract,
                                          null);

            var filteredFields = fields.Where(hasParamAttribute).ToArray();
            IEnumerable<Tuple<MemberInfo, Func<string, Object>, Func<Object, string>, int>> indexMapping;

            //if no param annotations are given, assume
            //that all members in declaration order are meant
            if (filteredFields.Length == 0)
            {
                filteredFields = fields.Where(m => hasIgnoreMemberAttribute(m) == false).ToArray();
                indexMapping = filteredFields.Select((m, i) => Tuple.Create(m, null as Func<string, Object>, null as Func<Object, string>, i + indexOffset));
            }
            else
            {
                indexMapping = filteredFields.Select(delegate(MemberInfo m)
                {
                    var param = (ParamNumberAttribute)m.GetCustomAttribute(typeof(ParamNumberAttribute));
                    return Tuple.Create(m, param.converterFuncIn, param.converterFuncOut, param.parameterIndex + indexOffset);
                });
            }
            return indexMapping.ToDictionary(t => t.Item4, t => Tuple.Create(t.Item1, t.Item2, t.Item3));
        }
        static private T Cast<T>(Object o)
        {
            return (T)o;
        }

        public static List<Type> findAllDerivedTypes<T>()
        {
            return findAllDerivedTypes<T>(Assembly.GetAssembly(typeof(T)));
        }

        public static List<Type> findAllDerivedTypes(Type T)
        {
            MethodInfo method = typeof(Marshaller).GetMethod("findAllDerivedTypes", new Type[] { typeof(Assembly) });
            MethodInfo generic = method.MakeGenericMethod(T);
            return (List<Type>)generic.Invoke(null, new object[] { Assembly.GetAssembly(T) });
        }

        public static List<Type> findAllDerivedTypes<T>(Assembly assembly)
        {
            var derivedType = typeof(T);
            try
            {
                //The code that causes the error goes here.
                return assembly
                    .GetTypes()
                    .Where(t =>
                        t != derivedType &&
                        derivedType.IsAssignableFrom(t)
                        ).ToList();
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    if (exSub is FileNotFoundException)
                    {
                        FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();
                //Display or log the error based on your application.
            }
            return null;
        }

        static private void mapToEFModel<From, To>(From from, To to, List<Tuple<Type,Type>> derivedTypes)
        {
            var maps = Mapper.GetAllTypeMaps();
            var tgtTypeMap = maps.ToList().Where(_ => _.SourceType == from.GetType()).First();
            derivedTypes = derivedTypes.Where(_ => 
                !tgtTypeMap.IncludedDerivedTypes.Select(tm => tm.SourceType).Contains(_.Item1)).ToList();
            if (derivedTypes.Count == 0) { return; }
            derivedTypes.ForEach(t => tgtTypeMap.IncludeDerivedTypes(t.Item1, t.Item2));
        }

        static private void mapToEFModel<From>(From from)
        {
            var to = ModelAssembly.GetTypes().Where(t => t.Name == from.GetType().Name);
            if (to.ToList().Count == 1 
                && from.GetType().Assembly == typeof(Marshaller).Assembly) 
            {
                Mapper.CreateMap(from.GetType(), to.First());
                var derivedTypesFrom = findAllDerivedTypes(from.GetType());
                if (derivedTypesFrom.Count > 0)
                {
                    var derivedTypesTo = findAllDerivedTypes(to.First());
                    if (derivedTypesTo.Count > 0)
                    {
                        Func<List<Type>, List<Type>, List<Tuple<Type, Type>>> matchDerivedTypes = 
                            delegate(List<Type> Source, List<Type> Destination)
                        {
                            return Source.Join(Destination,
                                src => src.Name,
                                dst => dst.Name,
                                (src, dst) => Tuple.Create(src, dst)).ToList();
                        };
                        mapToEFModel((From)Activator.CreateInstance(from.GetType()), to.First(),
                           matchDerivedTypes(derivedTypesFrom, 
                           derivedTypesTo));
                    }

                }
            }
        }


        static private string unmarshalFrom(ListBlock from)
        {
            if (!from.isValue())
            {
                throw new ArgumentException("Failed to marshal, from does not represent a string, and target is a string");
            }
            return from.value;
        }

        static private IList<string> unmarshalFromStringListToList(IList<ListBlock> from)
        {
            return from.Select(i => i.value).ToList();
        }

        static private IList<T> unmarshalFromListToList<T>(IList<ListBlock> from) where T : class
        {
            return (IList<T>)from.Select(i => unmarshalFrom<T>(i) as T).ToList();
        }

        static private T unmarshalFromBase<T>(ListBlock from) where T : class
        {
            return unmarshalFrom<T>(from);
        }

        static public T unmarshalFrom<T>(ListBlock from) where T : class
        {
            T result = (T)Activator.CreateInstance(typeof(T));
            Type type = typeof(T);
            bool outputIsList = typeof(IList).IsAssignableFrom(typeof(T));
            bool outputIsArray = type.IsArray;
            bool outputIsCollection = outputIsList || outputIsArray;
            bool inputIsArray = from.isArray();

            if (from == null || from.isEmpty())
            {
                if (outputIsCollection)
                {
                    return result;
                }
                return null;
            }

            //array ^= array
            //array ^= list
            if (outputIsCollection && inputIsArray)
            {
                Type elementType = null;

                if (type.IsGenericType)
                {
                    elementType = type.GetGenericArguments()[0];
                }

                if (elementType == typeof(string))
                {
                    return (T)unmarshalFromStringListToList(from.block);
                }
                else
                {
                    mapToEFModel(Activator.CreateInstance(elementType));
                    var resultList = dynamicCall("unmarshalFromListToList", elementType, new object[] { from.block });
                    return (T)resultList;
                }
            }
            //object ^= array
            else if (!outputIsCollection && inputIsArray)
            {
                //"["Assign..", ["class.."]]"
                //"["class"]
                if (
                    //Candidate<Derived:Item> vetting
                        from.block.Count == 2
                        && from.block[0].isValue()
                        && from.block[1].isArray()
                    )
                {
                    Type matchedType = null;
                    if (typeof(T).GetCustomAttribute(typeof(DerivedTypeAttribute)) != null)
                    {
                        matchedType = findAllDerivedTypes<T>().Where(t => t.Name == from.block[0].value).First();
                    }
                    else if (typeof(T).BaseType.GetCustomAttribute(typeof(DerivedTypeAttribute)) != null)
                    {
                        matchedType = findAllDerivedTypes(typeof(T).BaseType).Where(t => t.Name == from.block[0].value).First();
                    }

                    if (matchedType != null)
                    {
                        mapToEFModel(matchedType);
                        return (T)dynamicCall("unmarshalFromBase", matchedType, new object[] { from.block[1] });
                    }
                }
                mapToEFModel(result);
                return unmarshalObjectFrom<T>(from.block);
            }
            //object (one member) ^= scalar
            else if (!outputIsCollection && !inputIsArray)
            {
                return unmarshalObjectFrom<T>(new ListBlock[] { from });
            }
            // array ^= scalar -> invalid
            else if (outputIsCollection && !inputIsArray)
            {
                throw new ArgumentException("Failed to marshal, trying to combine array with scalar");
            }
            return result;
        }

        static private T unmarshalObjectFrom<T>(IList<ListBlock> from) where T : class
        {
            var dict = createParamNumberDictionaryWithInheritance(typeof(T));
            if (dict.Count != from.Count)
            {
                throw new ArgumentException("Failed to marshal, (case array -> object), unequal length, trying to consume " + dict.Count + " of " + from.Count, typeof(T).Name);
            }

            T result = (T)Activator.CreateInstance(typeof(T));
            if (from.Count == 0) { return result; }

            foreach (var pair in dict)
            {
                var key = pair.Key;
                var value = pair.Value;
                var MemberInfo = new ConversibleMemberInfo(value.Item1);
                var converter = MemberInfo.ConverterIn;
                var item = from[key];
                Action<Object, Object> setFunc = null;
                Type type = null;

                if (MemberInfo.isFieldInfo)
                {
                    var field = MemberInfo.FieldInfo;
                    setFunc = field.SetValue;
                    type = field.FieldType;
                }

                if (MemberInfo.isPropertyInfo)
                {
                    var property = MemberInfo.PropertyInfo;
                    setFunc = property.SetValue;
                    type = property.PropertyType;
                }

                Object newValue;

                //direct assignment
                if (item.isValue())
                {
                    newValue = MemberInfo.hasConverterIn 
                        ? MemberInfo.ConverterIn(item.value) 
                        : item.value;
                }
                else
                {
                    newValue = dynamicCall("unmarshalFromBase", type, new object[] { item });
                }

                try
                {
                    setFunc(result, newValue);
                }
                catch (Exception e)
                {
                    throw new ArgumentException("Failed to marshal", typeof(T).Name, e);
                }
            }
            return result;
        }

        static public T unmarshalFrom<T>(string stringRepresentation) where T : class
        {
            ListBlock block = explodeNested(stringRepresentation);
            return unmarshalFrom<T>(block);
        }


        static public string marshalFrom<T>(T source) where T : class
        {
            return null;
        }


        #region ReverseMarshalling Object -> ListBlock

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


        private class ConversibleMemberInfo 
        {
            public MemberInfo MemberInfo { get; private set; }
            public bool isFieldInfo { get; private set; }
            public bool isPropertyInfo { get; private set; }
            public PropertyInfo PropertyInfo { get; private set; }
            public FieldInfo FieldInfo { get; private set; }
            public Func<string, Object> ConverterIn { get; private set; }
            public Func<Object, string> ConverterOut { get; private set; }
            public bool hasConverterIn 
            { 
                get 
                { 
                    return ConverterIn != null; 
                } 
                private set 
                { 
                    hasConverterIn = value; 
                } 
            }

            public bool hasConverterOut 
            {
                get 
                { 
                    return ConverterOut != null; 
                } 
                private  set 
                {
                    hasConverterOut = value;
                } 
            }

            public ConversibleMemberInfo (MemberInfo MemberInfo, 
                bool UseDefaultConverter=true)
	        {
                this.MemberInfo = MemberInfo;
                if (MemberInfo.MemberType == MemberTypes.Property)
                {
                    isPropertyInfo = true;
                    isFieldInfo = false;
                    PropertyInfo = (PropertyInfo)MemberInfo;
                }
                else if (MemberInfo.MemberType == MemberTypes.Field)
                {
                    isPropertyInfo = false;
                    isFieldInfo = true;
                    FieldInfo = (FieldInfo)MemberInfo;
                }
                if (UseDefaultConverter)
                {
                    ConverterIn = TypeConverter.GetInputConverter(isPropertyInfo 
                        ? PropertyInfo.PropertyType 
                        : FieldInfo.FieldType);
                    ConverterOut = TypeConverter.GetOutputConverter(isPropertyInfo
                        ? PropertyInfo.PropertyType
                        : FieldInfo.FieldType);
                }
                else
                {
                    ConverterIn = null;
                    ConverterOut = null;
                }
	        }


        }

        public interface IConversionDictionary
        {
            IDictionary<Type, Tuple<Func<Object, string>, Func<string, Object>>> GetConversionDictionary();
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
                SetDefaultConverters();
            }

            private static void SetDefaultConverters()
            {
                Func<Object, string> outCon;
                Func<string, Object> inCon;
                Func<Func<Object,string>,Func<string,Object>,
                    Tuple<Func<Object,string>,Func<string,Object>>> New = 
                    delegate(Func<Object,string> oCon, Func<string,Object> iCon)
                    {
                        return Tuple.Create(oCon,iCon);
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

            public static void SetConverters(IConversionDictionary ConversionDictionary)
            {
                TypeConverter.ConversionDictionary = ConversionDictionary.GetConversionDictionary();
            }


            public static Func<string, Object> GetInputConverter(Type inputType)
            {
                Tuple<Func<Object,string>,Func<string,Object>> retVal;
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
        }

        private static class PrimitiveTypes
        {
            private static HashSet<Type> primitiveTypes;

            static PrimitiveTypes()
            {
                primitiveTypes = new HashSet<Type> 
                { 
                    typeof(String), 
                    typeof(Int16),
                    typeof(Int32),
                    typeof(Int64),
                };
            }

            public static bool IsPrimitive(Type Type)
            {
                return Type.IsPrimitive 
                    || primitiveTypes.Any(_ => _ == Type);
            }

        }



        private static TypeCheck inspectType<T>(T source,
                    ConversibleMemberInfo MemberInfo)
        {
            bool isScalar = false;
            bool isScalarCollection = false;
            bool isObjectCollection = false;

            if (typeof(IList).IsAssignableFrom(source as Type))
            {
                Type elementType = (source as Type).GetGenericArguments()[0];
                isScalarCollection = PrimitiveTypes.IsPrimitive(elementType) 
                    || TypeConverter.GetOutputConverter(elementType) != null;
                isObjectCollection = !isScalarCollection;
            }
            isScalar = PrimitiveTypes.IsPrimitive(source as Type);

            if (!isScalar && MemberInfo.hasConverterOut)
            {
                isScalar = true; // we can handle this as a scalar 
            }

            // infinite recursion for Non-User-Types w/o an assigned Converter that are not primitives
            bool isObject = !isScalar && !isScalarCollection && !isObjectCollection;
            var checkList = new List<bool> { isObject, isScalar, isScalarCollection, isObjectCollection };
            if (checkList.Where(_ => _ == true).Count() > 1)
            {
                throw new UndefinedMarshallingStateException(
                    String.Format(
                        "Processed Type {0} has ambiguous marshalling type. It may only resolve to a singular type.",
                        source as Type
                    ));
            }
            return new TypeCheck(isScalar, isScalarCollection, isObjectCollection, isObject);
        }

        /// <summary>
        /// Creates a ListBlock that reflects the nested hierarchy of an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns>ListBlock</returns>
        public static ListBlock marshalFromObject<T>(T source)
        {
            var returnBlock = new ListBlock();

            var tl_test = createParamNumberDictionaryWithInheritance(source.GetType());
            
            bool hasDerivedType = source.GetType().GetCustomAttribute<DerivedTypeAttribute>() != null
                || source.GetType().BaseType.GetCustomAttribute<DerivedTypeAttribute>() != null;
            ListBlock inner = null;
            if (hasDerivedType)
            {
                returnBlock.addElement(new ListBlock(source.GetType().Name));
                inner = new ListBlock();
            }

            foreach (var tpl in tl_test.OrderBy(_ => _.Key).ToList())
            {
                var mi = new ConversibleMemberInfo(tpl.Value.Item1);
                var type = inspectType((mi.isPropertyInfo ? mi.PropertyInfo.PropertyType : mi.FieldInfo.FieldType), mi);

                if (type.isScalar)
                {
                    if (!hasDerivedType)
                    {
                        returnBlock.addElement(marshalFromScalarMember(mi, source));
                    }
                    else
                    {
                        inner.addElement(marshalFromScalarMember(mi, source));
                    }
                }
                else if (type.isScalarCollection)
                {
                    if (!hasDerivedType)
                    {
                        returnBlock.addElement(new ListBlock(
                            marshalFromScalarMemberList(mi, source).ToList()));
                    }
                    else
                    {
                        inner.addElement(new ListBlock(
                            marshalFromScalarMemberList(mi, source).ToList()));
                    }
                }
                else if (type.isObject)
                {
                    var value = mi.isPropertyInfo 
                        ? mi.PropertyInfo.GetValue(source) 
                        : mi.FieldInfo.GetValue(source);
                    if (!hasDerivedType)
                    {
                        returnBlock.addElement(marshalFromObject(value));
                    }
                    else
                    {
                        inner.addElement(marshalFromObject(value));
                    }
                }
                else if (type.isObjectCollection)
                {
                    Type elementType = mi.isPropertyInfo
                        ? mi.PropertyInfo.PropertyType.GetGenericArguments()[0]
                        : mi.FieldInfo.FieldType.GetGenericArguments()[0];
                    if (!hasDerivedType)
                    {
                        returnBlock.addElement(new ListBlock(marshalFromObjectMemberList(
                            mi, source, Activator.CreateInstance(elementType)).ToList()));
                    }
                    else
                    {
                        inner.addElement(new ListBlock(marshalFromObjectMemberList(
                            mi, source, Activator.CreateInstance(elementType)).ToList()));
                    }
                }
            }
            if (hasDerivedType)
            {
                returnBlock.addElement(inner);
            }
            return returnBlock;
        }
        private static ListBlock marshalFromScalarMember<T>
            (ConversibleMemberInfo MemberInfo, T source)
        {
            var mi = MemberInfo;
            if (MemberInfo.isPropertyInfo)
            {
                try
                {
                    // TODO: sort out conversion (interface maybe for external config?)
                    string value = mi.hasConverterOut
                        ? mi.ConverterOut(mi.PropertyInfo.GetValue(source))
                        : mi.PropertyInfo.GetValue(source) as string;
                    return new ListBlock(value);
                }
                catch (Exception)
                {
                    if (!mi.hasConverterOut
                        && mi.PropertyInfo.PropertyType != typeof(string))
                    {
                        var t = mi.PropertyInfo.GetValue(source).GetType();
                        throw new MissingOutputConverterFunctionException
                            (String.Format("Cant marshal type without a conversion function from {0} to string", t.Name));
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else if (MemberInfo.isFieldInfo)
            {
                try
                {
                    string value = mi.hasConverterOut
                        ? mi.ConverterOut(mi.FieldInfo.GetValue(source))
                        : mi.PropertyInfo.GetValue(source) as string;
                    return new ListBlock(value);
                }
                catch (Exception)
                {
                    if (!mi.hasConverterOut
                        && mi.FieldInfo.FieldType != typeof(string))
                    {
                        var t = mi.FieldInfo.GetValue(source).GetType();
                        throw new MissingOutputConverterFunctionException
                            (String.Format("Cant marshal type without a conversion function from {0} to string", t.Name));
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return null;
        }

        private static ListBlock marshalFromScalar<T>(T Scalar)
        {
            var converter =
                TypeConverter.GetOutputConverter(Scalar.GetType());
            return new ListBlock(converter(Scalar));
        }

        private static IList<ListBlock> marshalFromScalarMemberList<T>
            (ConversibleMemberInfo MemberInfo, T source)
        {
            var mi = MemberInfo;
            var retList = new List<string>();
            var ret = new List<ListBlock>();
            if (mi.isPropertyInfo)
            {
                //TODO: add proper exception handling when missing conversion func
                Type elementType = mi.PropertyInfo.PropertyType.GetGenericArguments()[0];
                if (elementType == typeof(string))
                {
                    retList = mi.PropertyInfo.GetValue(source) as List<string>;
                }
                else
                {
                    return marshalFromScalarMemberList(
                        mi, source, Activator.CreateInstance(elementType)).ToList();
                }

            }
            else if (mi.isFieldInfo)
            {
                Type elementType = mi.FieldInfo.FieldType.GetGenericArguments()[0];
                if (elementType == typeof(string))
                {
                    retList = mi.FieldInfo.GetValue(source) as List<string>;
                }
                else
                {
                    return marshalFromScalarMemberList(
                        mi, source, Activator.CreateInstance(elementType)).ToList();
                }
            }
            else
            {
                return null;
            }
            retList.ForEach(_ => ret.Add(new ListBlock(_)));
            return ret;
        }

        private static IList<ListBlock> marshalFromScalarMemberList<T, Te>
            (ConversibleMemberInfo MemberInfo, T source, Te elemType)
        {
            var ret = new List<ListBlock>();
            var mi = MemberInfo;
            var srcList =
                 (
                     mi.isPropertyInfo
                     ? mi.PropertyInfo.GetValue(source) as IList
                     : mi.FieldInfo.GetValue(source) as IList
                 ).Cast<Te>().ToList();
            
            srcList.ForEach(_ => ret.Add(marshalFromScalar(_)));
            return ret;
        }

        private static IList<ListBlock> marshalFromObjectMemberList<T, Te>
            (ConversibleMemberInfo MemberInfo, T source, Te elemType)
        {
            // check for null
            var ret = new List<ListBlock>();
            var mi = MemberInfo;
            var srcList = 
                (
                    mi.isPropertyInfo 
                    ? mi.PropertyInfo.GetValue(source) as IList 
                    : mi.FieldInfo.GetValue(source) as IList
                ).Cast<Te>().ToList();
            srcList.ForEach(_ => ret.Add(marshalFromObject(_)));
            return ret;
        }


        public static string marshalFromListBlock(ListBlock ListBlock)
        {
            var retStr = String.Empty;

            Func<string, string> w = _ => "\"" + _ + "\"";

            if (ListBlock.isArray())
            {
                retStr = "[";
                int idx = 0;
                foreach (var lb in ListBlock.block)
                {
                    bool hasNext = ListBlock.block.Count > idx + 1;
                    idx++;
                    if (hasNext)
                    {
                        if (lb.isValue())
                        {
                            retStr += w(lb.value) + ",";
                        }
                        else if (lb.isArray())
                        {
                            retStr += marshalFromListBlock(lb) + ",";
                        }
                        else if (lb.isEmpty())
                        {
                            // ""
                            // []
                        }
                    }
                    else
                    {
                        if (lb.isArray())
                        {
                            retStr += marshalFromListBlock(lb);
                        }
                        else if (lb.isValue())
                        {
                            retStr += w(lb.value);
                        }
                    }
                }
                retStr += "]";
            }
            else if (ListBlock.isValue())
            {
                retStr = w(ListBlock.value);
            }
            else if (ListBlock.isEmpty())
            {

            }
            return retStr;
        }
        #endregion





    }
}