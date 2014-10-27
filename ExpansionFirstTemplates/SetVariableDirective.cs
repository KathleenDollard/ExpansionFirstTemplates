using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoslynDom.Common;

namespace ExpansionFirstTemplates
{
    public class SetVariableDirective : IKnownDirective
    {

        public string Identifier
        { get { return "SetVariable"; } }

        public bool HasAction
        { get { return false; } }

        public void Start<T>(T item, MetadataContextStack metaContextStack, IHasLookupValue data)
        {
            // TODO: At the moment, this idea is sound, but the implementation is pretty meaningless
            // TODO: Add a HasValue or GetValue system where you can have this as an optional value
            var getVarName = data.GetValue<string>("GetVarName");
            var varName = data.GetValue<string>("VarName");
            if (string.IsNullOrWhiteSpace(getVarName))
            { getVarName = varName; }
            var value = metaContextStack.GetValue(getVarName);
            metaContextStack.Push(varName, value);
        }

        public IEnumerable<T> Act<T>(T item, MetadataContextStack metaContextStack)
           where T : IDom
      {
            // No Action
            return null;
        }

        public void Finish<T>(T item, MetadataContextStack metaContextStack, IHasLookupValue data)
        {
            metaContextStack.Pop();
        }

    }
}
