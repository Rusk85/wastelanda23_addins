using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public class LoadedMagazines
    {

        public string ClassName { get; private set; }
        public int Bullets { get; private set; }

        public LoadedMagazines(string ClassName, int Bullets)
        {
            this.ClassName = ClassName;
            this.Bullets = Bullets;
        }


    }
}
