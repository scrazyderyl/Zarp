namespace Zarp.Core.Datatypes
{
    public interface Preset
    {
        public string Name { get; set; }

        public Preset Duplicate(string name);
    }
}
