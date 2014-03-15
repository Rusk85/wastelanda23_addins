using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling
{
    public struct Position
    {
        public Position(int value) { Value = value; }
        public int Value;
    }


    public struct Level
    {
        public Level(int value) { Value = value; }
        public int Value;
    }


    public struct Range
    {
        public int Level;
        public int Start;
        public int End;
        public int Length;
    }


    public struct Element
    {
        public string ClassName;
        public int Level;
        public int Position;
    }

}
