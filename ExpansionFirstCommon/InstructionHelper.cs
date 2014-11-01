using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoslynDom.Common;

namespace ExpansionFirst.Common
{
   public class InstructionHelper
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

      public void RunOneLoop(IEnumerable<IDom> blockContents, IDetailBlockStart blockStart,
                     MetadataContextStack contextStack, List<IDom> newList)
      {
         var expansionFirstRunner = contextStack.GetValue(Constants.ExpansionFirstRunner) as ExpansionFirstTemplate;
         var member = blockContents.FirstOrDefault();
         var i = 0;
         while (member != null && blockContents.Contains(member))
         {
            i++; if (i > 1000) throw new InvalidOperationException("Infinite loop detected");
            var lastMember = member;
            var copiedMember = member.GetType().GetMethod("Copy").Invoke(member, null) as IDom;
            var newMembers = expansionFirstRunner.Update(copiedMember, contextStack, ref lastMember);
            // don't store the end region that matches this block start, because it's being removed
            newMembers = newMembers
                           .Where(x =>
                           {
                              var block = x as IDetailBlockEnd;
                              if (block == null) return true;
                              return (block.GroupGuid != blockStart.GroupGuid);
                           });
            newList.AddRange(newMembers);
            member = lastMember.NextSibling();
         }
      }
   }
}
