using System;
using ExpansionFirst.Common;
using RoslynDom.Common;
using System.Text.RegularExpressions;

namespace ExpansionFirstTemplates
{
   public class ReplacementAlteration : IAlteration
   {
      public string Id
      { get { return "Replacement"; } }

      private Regex nameRegex = new Regex("_xf_(/w+)");

      public void DoAlteration(IDom item, MetadataContextStack contextStack)
      {
         var partHasName = item as IHasName;
         if (partHasName != null)
         {
            partHasName.Name = ReplacementString(partHasName.Name, contextStack);
         }
         // Replace expressions

      }

      private string ReplacementString(string input, MetadataContextStack contextStack)
      {
         var key = input.SubstringAfter("_xf_");
         if (string.IsNullOrWhiteSpace(key)) { return input; }
         object value;
         var ret = input;
         if (contextStack.TryGetValue(key, out value))
         {
            ret = input.Replace("_xf_" + key, value.ToString());
         }
         return ret;
      }
   }
}
