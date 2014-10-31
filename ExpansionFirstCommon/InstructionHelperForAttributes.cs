using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoslynDom.Common;

namespace ExpansionFirst.Common
{
   public class InstructionHelperForAttributes
   {
      public IEnumerable<IAttribute> GetMatchingAttributes(IDom item, string id)
      {
         var itemHasAttributes = item as IHasAttributes;
         if (itemHasAttributes == null) return new List<IAttribute>();
         var attributes = itemHasAttributes.Attributes;
         var xfAttributes = attributes
                           .Where(x =>
                           {
                              if (!x.Name.StartsWith("_xf_.")) return false;
                              var name = x.Name.SubstringAfter("_xf_.");
                              return (!string.IsNullOrWhiteSpace(name) && name == id);
                           });
         return xfAttributes;
      }

      public T GetBestMetadata<T>(IAttribute attribute, MetadataContextStack contextStack)
         where T : class
      {
         var attributeValue = attribute.AttributeValues.FirstOrDefault();
         T value = null;
         if (attributeValue != null)
         {
            var metaName = attributeValue.Value.ToString();
            value = contextStack.GetValue(metaName) as T;
         }
         else
         {
            // Convention
            var tuple = contextStack.RecentOfType<T>();
            if (tuple != null)
            { value = contextStack.RecentOfType<T>().Item2; }
         }
         return value;
      }
   }
}
