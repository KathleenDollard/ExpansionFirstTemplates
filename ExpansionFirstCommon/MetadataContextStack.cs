using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoslynDom.Common;

namespace ExpansionFirstTemplates
{
    public class MetadataContextStack : IHasLookupValue
    {
        private Stack<MetadataContext> metadataStack = new Stack<MetadataContext >();

        public MetadataContext Current
        { get { return metadataStack.Peek(); } }

        public void Push(MetadataContext context)
        { metadataStack.Push(context); }

        public void Push(string name, object item)
        {
            var newContext = new MetadataContext();
            newContext.AddValue(name, item);
            Push(newContext);
        }

      public void Add(string name, object item)
      {
         Current.AddValue(name, item);
      }



      public MetadataContext Pop()
        { return metadataStack.Pop(); }

        public object GetValue(string name)
        {
            foreach (var item in metadataStack)
            {
                object value;
                if (item.TryGetValue(name, out value)) { return value; }
            }
            // TODO: Log or otherwise report, possibly configuration to throw excpetion here
            return null;
        }

        public string GetValueAsString(string name)
        { return GetValue(name).ToString(); }

        public string GetValueAsQuotedString(string name)
        { return "" + GetValue(name).ToString() + ""; }

        public T GetValue<T>(string key)
        {
            var item = GetValue(key);
            if (item is T) return (T)item;
            // TODO: Log or otherwise report, possibly configuration to throw excpetion here
            return default(T);
        }

        public bool HasValue(string key)
        {
            foreach (var item in metadataStack)
            { if (item.HasValue(key)) { return true; } }
            return false;
        }

        public bool TryGetValue(string key, out object value)
        {
            value = null;
            if (!HasValue(key)) { return false; }
            value = GetValue(key);
            return true;
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            value = default(T);
            if (!HasValue(key)) { return false; }
            value = GetValue<T>(key);
            return true;
        }


    }
}
