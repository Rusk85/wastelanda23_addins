using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;

namespace WastelandA23.Marshalling
{
    public class Marshaller
    {
        private string SQFString;
        private char[] str;
        private Dictionary<int, List<Range>> LevelRange = new Dictionary<int, List<Range>>();
        private Dictionary<int, List<Position>> LevelPosition = new Dictionary<int, List<Position>>();
        private List<Element> Elements = new List<Element>();
        private string debugStr = "[[\"SAVE_COMMAND\",\"765611979,64280320\"],[[\"ItemMap\",\"ItemCompass\",\"ItemWatch\",\"H_MilCap_mcamo\",\"G_Tactical_Black\"],\"\",[],\"Colt1911\",[\"\",\"\",\"\"],\"\",[],\"U_B_CombatUniform_mcam\",[\"7Rnd_45ACP_1911\"],\"V_BandollierB_rgr\",[],\"B_Carryall_cbr\",[],[[],[\"7Rnd_45ACP_1911\"],[],[\"7Rnd_45ACP_1911\",\"7Rnd_45ACP_1911\",\"7Rnd_45ACP_1911\",\"7Rnd_45ACP_1911\",\"7Rnd_45ACP_1911\"]],\"Colt1911\",\"Colt1911\"]]";
        
        public Marshaller(string SQFString) 
        {
            this.SQFString = SQFString;
            str = SQFString.ToCharArray();
            #if DEBUG
            this.SQFString = debugStr;
            str = this.SQFString.ToCharArray();
            #endif
        }


        public void startWalking()
        {
            walk(0,str.Length, 0);
            doRangeParsing();
        }


        private void walk(int curPos, int length, int level)
        {
            int balanced = 0;
            int start = 0;
            int end = 0;
            level++;

            for (int i = curPos; i < length; i++)
            {

                if (str[i] == '[')
                {
                    if (balanced++ == 0) { start = i; }
                }
                else if (str[i] == ']')
                {
                    if (--balanced == 0) { end = i; }
                }

                if (balanced == 0 && (str[i] == ']' || str[i] == '['))
                {
                    addLevelRange(level, start, end);
                    walk(++start, end, level);
                }
            }
        }


        private void doRangeParsing()
        {
            foreach (var l in LevelRange.Values)
            {
                foreach (Range r in l){ parseRange(r); }
            }
        }


        private void parseRange(Range range)
        {
            var s = SQFString.Substring(range.Start, range.Length).ToCharArray();
            int balanced = 0;
            bool foundBracket = false;
            int start = 0;
            int end = 0;

            List<Range> nests = new List<Range>();

            // remove brackets at the end and the beginning
            if (s[0] == '[') s[0] = '#';
            if (s[s.Length - 1] == ']') s[s.Length - 1] = '#';
            s = new String(s).Replace("#", "").ToCharArray();
            var pos = getLevelPosition(range.Level);

            // lets find possible additional nested arrays in this particular array
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '[')
                {
                    if (balanced++ == 0) { start = i; foundBracket = true; }
                }
                else if (s[i] == ']')
                {
                    if (--balanced == 0) { end = i; }
                }
                else
                {
                    continue; // nothing to see or do here
                }

                if (balanced == 0 && foundBracket)
                {
                    nests.Add(new Range() { Start = start, End = end, Length = end - start });
                    foundBracket = false;
                }
            }

            // mark nested arrays for deletion
            foreach (Range r in nests)
            {
                for (int i = r.Start; i < r.End; i++)
                {
                    s[i] = '#';
                }
            }

            // execute deletion
            var prelim = new String(s).Replace("#", String.Empty);

            var elemList = new List<Element>();
            foreach (string eStr in split(prelim))
            {
                var r = "";
                foreach (string rep in new String[] { "[", "]" })
                {
                    r = eStr.Replace(rep, String.Empty);
                }
                if (r != String.Empty) { elemList.Add(new Element() { ClassName = r, Level = range.Level, Position = pos.Value }); }
            }

            #if DEBUG
            foreach (Element e in elemList)
            {
                Debug.WriteLine(String.Format("ClassName {0} | Level {1} | Position {2}", e.ClassName, e.Level, e.Position));
            }
            #endif
            if (elemList.Count == 0) { return; }
            Elements = Elements.Concat(elemList).ToList();
        }


        private string[] split(string Str)
        {
            bool balanced = true;
            bool foundQM = false;
            int start = 0;
            int end = 0;
            var cStr = Str.ToCharArray();

            var ranges = new List<Range>();

            for (int i = 0; i < cStr.Length; i++)
            {
                if (cStr[i] == '"' && balanced && !foundQM)
                {
                    balanced = false;
                    foundQM = true;
                    start = i;
                }
                else if (cStr[i] == '"' && !balanced)
                {
                    balanced = true;
                    end = i;
                }
                else if (cStr[i] == '"' && balanced)
                {
                    balanced = false;
                    start = i;
                }

                if (balanced && foundQM)
                {
                    ranges.Add(new Range() { Start = start, End = end, Length = end - start });
                    foundQM = false;
                }
            }

            var eList = new List<string>();
            foreach (Range r in ranges)
            {
                eList.Add(Str.Substring(r.Start, r.Length).Replace("\"",""));
            }
            return eList.ToArray();
        }


        private void addLevelRange(int Level, int Start, int End)
        {
            End++;
            #if DEBUG
            Debug.WriteLine(String.Format("Start {0} | End {1} | Substring {2}", 
                Start, End, SQFString.Substring(Start, End - Start)));
            #endif
            if (LevelRange.ContainsKey(Level))
            {
                LevelRange[Level].Add(new Range() {Level=Level, Start=Start, End=End, Length = End-Start });
            }
            else
            {
                var l = new List<Range>();
                l.Add(new Range() { Level = Level, Start = Start, End = End, Length = End-Start });
                LevelRange.Add(Level, l);
            }
        }


        private Position getLevelPosition(int Level)
        {
            var p = new Position(Level);
            if (LevelPosition.ContainsKey(Level))
            {

                if (LevelPosition[Level].Count == 0)
                {
                    p.Value = 0;
                }
                else
                {
                    var pos = LevelPosition[Level].Max(x => x.Value);
                    p.Value = ++pos;
                }
                LevelPosition[Level].Add(p);
            }
            else
            {
                LevelPosition.Add(Level, new List<Position> { new Position(0) });
                p.Value = 0;
            }
            return p;
        }

        public class ListBlock
        {
            public ListBlock() { }
            public ListBlock(string value)
            {
                this.value = value;
            }

            public ListBlock(List<ListBlock> block)
            {
                this.block = block;
            }

            public void addElement(ListBlock block) {
                if (this.block == null)
                {
                    this.block = new List<ListBlock>();
                }
                this.block.Add(block);
            }

            public bool isEmpty() { return !isValue() && !isArray(); }
            public bool isValue() { return value != null;  }
            public bool isArray() { return block != null;  }

            public string value = null;
            public List<ListBlock> block = null;
        }


        
        //static string test = "[[SAVE_COMMAND,76561197964280320],[1.2, 3.4]]";

        static List<string> explodeIfNotEscaped(string str, char blockStart, char blockEnd, char delimiter)
        {
            List<string> result = new List<string>();

            if (String.IsNullOrEmpty(str))
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
                    result.Add(sub);
                    start = i + 1;
                }
            }
            return result;
        }

        static string trimOnce(string str, string beginString, string endString)
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

        static public ListBlock explodeNested(string str)
        {
            ListBlock result = new ListBlock();

            if (String.IsNullOrEmpty(str))
            {
                result.value = "";
                return result;
            }

            str = trimOnce(str, "[", "]");

            if (!str.StartsWith("[") && !str.EndsWith("]"))
            {
                var valueList = explodeIfNotEscaped(str, '\"', '\"', ',');
                result.block = valueList.Select(f => new ListBlock(f)).ToList();
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
                    else
                    {
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
                return dictBase.Union(dictThis).ToDictionary (k => k.Key, v => v.Value);
            }
        }

        static public IDictionary<int, Tuple<MemberInfo, Func<string, Object>, Func<Object, string>>> createParamNumberDictionaryWithInheritance(Type type)
        {
            return createParamNumberDictionaryWithInheritance(type, 0);
        }

        static public IDictionary<int, Tuple<MemberInfo, Func<string, Object>, Func<Object, string>>> createParamNumberDictionary(Type type, int indexOffset = 0)
        {
            Func<MemberInfo, bool> hasParamAttribute = delegate(MemberInfo m) { return m.GetCustomAttribute(typeof(ParamNumberAttribute)) != null; };
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
                filteredFields = fields;
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

        static public T unmarshalFrom<T>(ListBlock from) where T: class
        {
            T result = (T)Activator.CreateInstance(typeof(T)); 
            if (from == null || from.isEmpty())
            {
                return result;
            }

            Type type = typeof(T);
            bool outputIsList = typeof(IList).IsAssignableFrom(typeof(T));
            bool outputIsArray = type.IsArray;
            bool outputIsCollection = outputIsList || outputIsArray;
            bool inputIsArray = from.isArray();

            //array ^= array
            //array ^= list
            if (outputIsCollection && inputIsArray)
            {
                Type elementType = null;

                if (type.IsGenericType)
                {
                    elementType = type.GetGenericArguments()[0];
                }

                if (elementType == typeof(string)) {
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

        static public T unmarshalFrom<T>(string stringRepresentation) where T: class
        {
            ListBlock block = explodeNested(stringRepresentation);
            return unmarshalFrom<T>(block);
        }
    }
}
