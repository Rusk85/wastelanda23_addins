using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public class LoadedMagazines
    {

        public string ClassName { get; set; }
        public string Bullets { get; set; }

        public LoadedMagazines(string ClassName, string Bullets)
        {
            this.ClassName = ClassName;
            this.Bullets = Bullets;
        }

        public LoadedMagazines() { }


    }
}
