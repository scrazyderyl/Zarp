using System;
using System.Collections.Generic;

namespace Zarp.Core.Datatypes
{
    internal interface IDependency
    {
        public string Type { get; }
        public string DisplayName { get; }
        public Guid Guid { get; }
        IEnumerable<IDependency>? Dependencies { get; }
        IEnumerable<IDependency>? Dependents { get; }
    }
}
