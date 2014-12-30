using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoslynDom.Common;
using System.Text.RegularExpressions;

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

      public Regex BuildSpecificParamRegex(string instructionName, params string[] expectedParams)
      {
         var paramPattern = @"\s*{0}\s*[=:]\s*\""(?<{0}>.*)\""\s*";
         if (!instructionName.StartsWith(Constants.Prefix)) instructionName = Constants.Prefix + instructionName;
         var regexString = instructionName + @"\s*\(";
         var last = expectedParams.Count();
         for (int i = 0; i < last; i++)
         {
            regexString += string.Format(paramPattern, expectedParams[i]) + ",";
         }
         regexString = regexString.Substring(0, regexString.Length - 1) + @"\)";
         return new Regex(regexString);
      }

      public Regex BuildGeneralParamRegex(string instructionName)
      {
         var paramPattern = @"\s*(.*)\s*[=:]\s*\(.*)\s*";
         if (!instructionName.StartsWith(Constants.Prefix)) instructionName = Constants.Prefix + instructionName;
         var regexString = instructionName + @"\s*\(" + paramPattern +  @"\)";
         return new Regex(regexString);
      }

      public string PushToContext(string key, object value, MetadataContextStack contextStack)
      {
         contextStack.Current.AddValue(key, value);
         return key;
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

      public string MakeMarker(string id)
      { return Constants.Prefix + id; }

      public T Copy<T>(IDom part)
         where T : class
      {
         // Copy is an IDom<T> method, and I only have the part as an IDom, thus reflection
         var method = part.GetType().GetMethod("Copy");
         var newItem = method.Invoke(part, null) as T;
         return newItem;
      }
   }
}
