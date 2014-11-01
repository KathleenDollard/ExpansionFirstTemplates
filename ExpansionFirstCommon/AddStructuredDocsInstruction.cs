using System.Collections.Generic;
using RoslynDom.Common;
using System;
using CodeFirst.Common;

namespace ExpansionFirst.Common
{

   public partial class _xf_
   {
      public class AddAttributesAttribute : Attribute
      {
      }

      public class AddStructuredDocsAttribute : Attribute
      {
         public AddStructuredDocsAttribute()
         { }
         public AddStructuredDocsAttribute(string sourceName)
         { SourceName = sourceName; }

         public string SourceName { get; private set; }
      }
   }

   public class AddStructuredDocsInstruction : IInstruction
   {
      private InstructionHelper helper = new InstructionHelper();

      public string Id
      { get { return "AddStructuredDocs"; } }

      public bool DoInstruction(IDom part,
                   MetadataContextStack contextStack,
                   List<IDom> retList,
                   ref IDom lastPart,
                   ref bool reRootTemplate)
      {
         // always return false, even if this does somethign, it doesn't take care of the underlying part
         var candidates = helper.GetMatchingAttributes(part, Id);
         foreach (var attribute in candidates)
         {
            if (HandleAttribute(attribute, part as IHasAttributes, contextStack, retList, ref lastPart)) return false;
         }
         return false;
      }

      private bool HandleAttribute(IAttribute attribute, IHasAttributes item, MetadataContextStack contextStack, List<IDom> newList, ref IDom lastPart)
      {
         var value = helper.GetBestMetadata<CodeFirstMetadata>(attribute, contextStack);
         var targetHasStructuredDocs = item as IHasStructuredDocumentation;
         if (value != null && targetHasStructuredDocs != null && !string.IsNullOrWhiteSpace(value.XmlCommentString))
         {
            // targetHasStructuredDocs.StructuredDocumentation = value.XmlCommentString ;
         }
         return false;
      }
   }
}
