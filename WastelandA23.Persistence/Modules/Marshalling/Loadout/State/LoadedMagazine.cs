using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Marshalling.Loadout
{
    public class LoadedMagazine
    {

        public string ClassName { get; set; }
        public string Bullets { get; set; }

        public LoadedMagazine(string ClassName, string Bullets)
        {
            this.ClassName = ClassName;
            this.Bullets = Bullets;
        }

        public LoadedMagazine() { }


    }
}
