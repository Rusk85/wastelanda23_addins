using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                if (r != String.Empty){ elemList.Add(new Element() { ClassName = r, Level = range.Level, Position = pos.Value }); }
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


    }
}
