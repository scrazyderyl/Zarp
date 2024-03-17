using System;
using System.Collections.Generic;

namespace Zarp.Core.Datatypes
{
    internal abstract class Preset : IDependency
    {
        public readonly Guid Guid;
        internal string _Name;

        public abstract bool IsModifiable { get; }

        public abstract bool IsDeletable { get; }

        public abstract Preset Duplicate(string name);

        public Preset(string name)
        {
            Guid = Guid.NewGuid();
            _Name = name;
        }

        internal Preset(Guid guid, string name)
        {
            Guid = guid;
            _Name = name;
        }

        public override string ToString() => _Name;

        public abstract string Type { get; }

        public string DisplayName => _Name;

        Guid IDependency.Guid => Guid;

        public abstract IEnumerable<IDependency>? Dependencies { get; }

        public abstract IEnumerable<IDependency>? Dependents { get; }

    }
}
