namespace Zarp.Core.Datatypes
{
    internal interface IPreset
    {
        public string Name { get; set; }

        public IPreset Duplicate(string name);
    }
}
