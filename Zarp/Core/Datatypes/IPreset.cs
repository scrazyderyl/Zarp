namespace Zarp.Core.Datatypes
{
    public interface IPreset
    {
        public string Name { get; set; }

        public IPreset Duplicate(string name);
    }
}
