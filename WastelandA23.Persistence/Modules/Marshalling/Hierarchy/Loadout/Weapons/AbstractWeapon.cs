namespace WastelandA23.Marshalling.Loadout
{
    public abstract class AbstractWeapon
    {
        public string ClassName { get; private set; }

        public abstract Magazine LoadedMagazine { get; set; }
    }
}