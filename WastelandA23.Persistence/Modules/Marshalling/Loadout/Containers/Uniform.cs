using System.Collections.Generic;

namespace WastelandA23.Marshalling.Loadout
{
    public class Uniform : AbstractContainer
    {
        public List<Item> Items { get; private set; }

        public Uniform()
        {
        }
    }
}