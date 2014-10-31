using RoslynDom.Common;
using ExpansionFirst.Common;

namespace ExpansionFirstTemplates
{
   public static class XfHelpers
   {
      public static void UpdateIdentifiers(IDom item, string markingFormat, MetadataContextStack metaContextStack)
      {

      }

      public static void UpdateIdentifier(IDom item, string markingFormat, MetadataContextStack metaContextStack)
      {
         // TODO: Switch to RegEx groups to support non-prefix formats
         // TODO: Include comments, structured documentations and public annotations
         var itemHasName = item as IHasName;
         if (itemHasName.Name.StartsWith(markingFormat))
         {
            var localName = itemHasName.Name.SubstringAfter(markingFormat);
            if (localName.EndsWith("_wq"))
            {
               localName = localName.SubstringBefore("_wq");
               if (metaContextStack.HasValue(localName))
               { itemHasName.Name = metaContextStack.GetValueAsQuotedString(localName); }
            }
            else
            {
               if (metaContextStack.HasValue(localName))
               { itemHasName.Name = metaContextStack.GetValueAsString(localName); }
            }
         }
      }
   }
}
