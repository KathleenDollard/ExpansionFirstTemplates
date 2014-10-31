using System.Collections.Generic;
using RoslynDom.Common;
using ExpansionFirst.Common;

namespace ExpansionFirstTemplates
{
   public interface IKnownDirective
    {
        string Identifier { get; }
        bool HasAction { get; }
        void Start<T>(T item, MetadataContextStack meta, IHasLookupValue data);
        IEnumerable<T> Act<T>(T item, MetadataContextStack metaContextStack)
         where T : IDom;
        void Finish<T>(T item, MetadataContextStack meta, IHasLookupValue data);
    }
}
