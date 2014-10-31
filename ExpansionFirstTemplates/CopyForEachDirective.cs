using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RoslynDom.Common;
using ExpansionFirst.Common;

namespace ExpansionFirstTemplates
{
    public class CopyForEachDirective : IKnownDirective
    {
        private string _loopOver;
        private string _loopVarName;

        public  string Identifier
        { get { return "CopyForEach"; } }

        public bool HasAction
        { get { return true; } }

        public void Start<T>(T item, MetadataContextStack metaContextStack, IHasLookupValue data)
        {
            _loopOver = data.GetValue<string>("LoopOver");
            _loopVarName = data.GetValue<string>("LoopVarName");
        }

        public  IEnumerable<T> Act<T>(T item, MetadataContextStack metaContextStack)
            where T : IDom
        {
            var loopOver = metaContextStack.GetValue(_loopOver);
            var loopOverAsEnumerable = loopOver as IEnumerable;
            var ret = new List<T>();
            // TODO: Log, also consider configuration to throw here
            if (loopOverAsEnumerable == null) return null; 
            foreach (var loopItem in loopOverAsEnumerable )
            {
                metaContextStack.Push(_loopVarName, loopItem);
                var methodInfo = item
                            .GetType().GetTypeInfo()
                            .GetMethods()
                            .Where(x => x.Name == "Copy")
                            .FirstOrDefault();
                if (methodInfo == null) { } // TODO: Log 
                else
                {
                    var newItem = methodInfo.Invoke(item, null);
                    if (!(newItem is T)){ } // TODO: Log
                    else
                    {
                        var newItemAsT = (T)newItem;
                        XfHelpers. UpdateIdentifiers(newItemAsT, "_xf_", metaContextStack );
                        ret.Add(newItemAsT);
                    }
                }
                metaContextStack.Pop();
            }
            return ret;
        }

        public  void Finish<T>(T item, MetadataContextStack metaContextStack, IHasLookupValue data)
        {
            // No Cleanup to do here
        }
             
    }
}
