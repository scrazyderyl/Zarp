namespace Zarp.Core.Datatypes
{
    public abstract class Preset
    {
        public string Name;

        public Preset(string name)
        {
            Name = name;
        }

        public abstract Preset Duplicate(string name);

        public override string ToString()
        {
            return Name;
        }
    }
}
