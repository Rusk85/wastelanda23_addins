using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling
{
    public class Marshaller
    {

        private string testee = "[[b0,b1],[[c2],[c3]],a4,[b5,b6,b7]]";


        public Marshaller() { }


        public void doWalk()
        {
            Int16 zero = 0;
            walk(0, ref zero, testee.ToCharArray());
        }


        /// <summary>
        /// Recurses over a nested rxpression such as : [[b0,b1],[[c2],[c3]],a4,[b5,b6,b7]]
        /// While traversing the expression hierarchy and relative position of elements per hierarchy are being recorded.
        /// </summary>
        /// <param name="curPos">Global position for the whole of the param "str"</param>
        /// <param name="level">Recursion depth or level</param>
        /// <param name="str">String to parse</param>
        /// <returns></returns>
        //TODO: Add element parsing method 
        //      Look into behaviour for standalone elements such as [[],standaloneElement,[],[]]
        //      Create data structure that supports the element as such along with all of its attributes: level, pos
        //      Map to the AbstractMarshalling hierarchy with all its subclasses
        //      Create db model to reflect the hierarchy introduced with marshalling
        int walk(int curPos, ref Int16 level, char[] str)
        {
            if(curPos == 0){level = -1;}

            for (int i = curPos; i < str.Length; i++)
            {
                Console.WriteLine("LOOP-HEAD: " + str[i]);
                if (str[i] == '[')
                {
                    // increase recursion level
                   level++;
                   i = walk(++i, ref level, str);
                }
                else if (str[i] == ',')
                {
                    Console.WriteLine("',': " + str[i] + " " + str[i+1]);
                    if (str[i + 1] == '[') { continue; }    // skip iteration; will be dealt with on the next pass
                    else if (str[i + 1] == ',') { continue; } // 2+ commas in a row; record the position and skip iteration
                    else if (str[i + 1] == ']') { continue; } // skip iteration; will be dealt with on the next pass
                    else { continue; } //skip iteration; will be dealt with on the next pass
                }
                else if (str[i] == ']')
                {
                    // leaving of an element node; decrease recursion level && return curPos
                    --level;  
                    return i;
                }
                else
                {
                    // actual element parsing here
                    // parse and return adjusted curPos
                    // CASES:
                    // I. element,
                    // II. element]
                    // do we care for the cases 
                    // or can we let the loop take care of it ?
                    Console.WriteLine("Else: " + str[i]);
                }

            }
            return 0;
        }


    }
}
