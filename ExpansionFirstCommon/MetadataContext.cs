using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Reflection;

namespace ExpansionFirstTemplates
{
    public class MetadataContext : IHasLookupValue
    {
        private List<MetadataDefinition> _metadataDefintions = new List<MetadataDefinition>();

        public void AddValue(string name, object value)
        {
            _metadataDefintions.Add(new MetadataDefinition(name, value));
        }

        public bool TryGetValue(string name, out object value)
        {
            // TODO: This should iterate or recurse - currently hacked for testing
            var propName = "";
            if (name.Contains("__"))
            {
                name = name.Replace("__", ".");
                propName = name.SubstringAfter(".");
                name = name.SubstringBefore(".");
            }

            foreach (var def in _metadataDefintions)
            {
                if (def.Name == name)
                {
                    value = def.Value;
                    if (!string.IsNullOrWhiteSpace(propName))
                    { value = GetProperty(value, propName); }
                    return true;
                }
            }
            value = null;
            return false;
        }

        private object GetProperty(object value, string propName)
        {
            string tailPropName = propName;
            while (tailPropName.Contains("."))
            {
                propName = tailPropName.SubstringBefore(".");
                tailPropName = tailPropName.SubstringAfter(".");
                var propInfo = value
                            .GetType()
                            .GetProperties()
                            .Where(x => x.Name == propName)
                            .FirstOrDefault();
                if (propInfo == null) { return null; }
                { value = propInfo.GetValue(value); }
            }
            var propInfo2 = value
                     .GetType().GetTypeInfo()
                     .GetProperties()
                     .Where(x => x.Name == propName)
                     .FirstOrDefault();
            if (propInfo2 == null) { return null; }
            return propInfo2.GetValue(value); 
        }

        public object GetValue(string key)
        {
            object value;
            if (TryGetValue(key, out value)) return value;
            throw new NotImplementedException();
        }

        public T GetValue<T>(string key)
        {
            object value = GetValue(key);
            if (value is T) return (T)value;
            throw new NotImplementedException();
        }

        public bool HasValue(string key)
        {
            foreach (var def in _metadataDefintions)
            { if (def.Name == key) return true; }
            return false;
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            if (TryGetValue(key, out value)) return true;
            return false;
        }
    }
}
