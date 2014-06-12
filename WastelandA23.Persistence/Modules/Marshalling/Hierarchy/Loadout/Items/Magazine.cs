namespace WastelandA23.Marshalling.Loadout
{
    public class Magazine : Item
    {
        // TODO: string -> int
        [ParamNumber(0)]
        public int Bullets { get; set; }

        public Magazine()
            : base()
        {
        }
    }
}