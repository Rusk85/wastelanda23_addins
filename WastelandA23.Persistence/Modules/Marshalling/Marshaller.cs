using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Data.Entity;
using AutoMapper;

namespace WastelandA23.Marshalling
{
    public class Marshaller
    {

        public static Assembly ModelAssembly = Assembly.GetAssembly(typeof(WastelandA23.Model.CodeFirstModel.LoadoutContext));

        public class ListBlock
        {
            public ListBlock()
            {
            }

            public ListBlock(string value)
            {
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
                return value != null;
            }

            public bool isArray()
            {
                return block != null;
            }

            public string value = null;
            public List<ListBlock> block = null;
        }

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

        static private string unmarshalFrom(ListBlock from)
        {
            if (!from.isValue())
            {
                throw new ArgumentException("Failed to marshal, from does not represent a string, and target is a string");
            }
            return from.value;
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

        static private void mapToEFModel<From>(From from)
        {
            //TODO: Mapping for inheritance:
            //https://github.com/AutoMapper/AutoMapper/wiki/Mapping-inheritance
            //http://stackoverflow.com/questions/11264455/automapper-with-base-class-and-different-configuration-options-for-implementatio
            var to = ModelAssembly.GetTypes().Where(t => t.Name == from.GetType().Name);
            if (to.ToList().Count == 0) { return; }
            if (to.ToList().Count > 1) { to = to.OrderByDescending(t => t.Namespace.Length); }
            Mapper.CreateMap(from.GetType(), to.First());
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

                    if (typeof(T).BaseType.GetCustomAttribute(typeof(DerivedTypeAttribute)) != null)
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
                var memberInfo = value.Item1;
                var converter = value.Item2;
                var item = from[key];
                Action<Object, Object> setFunc = null;
                Type type = null;

                if (memberInfo is FieldInfo)
                {
                    var field = memberInfo as FieldInfo;
                    setFunc = field.SetValue;
                    type = field.FieldType;
                }

                if (memberInfo is PropertyInfo)
                {
                    var property = memberInfo as PropertyInfo;
                    setFunc = property.SetValue;
                    type = property.PropertyType;
                }

                Object newValue;

                //direct assignment
                if (item.isValue())
                {
                    Dictionary<Type, Func<string, Object>> bla = new Dictionary<Type, Func<string, Object>> {
                        {typeof(int), (s) => ((Object)Convert.ToInt32(s))}
                    };
                    if (converter == null) { bla.TryGetValue(type, out converter); };
                    newValue = (converter == null) ? (item.value) : (converter(item.value));
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
    }
}