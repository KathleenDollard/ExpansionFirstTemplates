using System.Collections.Generic;
using RoslynDom.Common;
using RoslynDom.RoslynDomUtilities;
using System.Text.RegularExpressions;
using ExpansionFirst.Common;
using System.Linq;
using System.Collections;
using RoslynDom;
using System;
using ExpansionFirstTemplates;
using CodeFirst.Common;

namespace ExpansionFirst
{
   namespace TemplateSupport
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
   }

   public class AddStructuredDocsInstruction : IInstruction
   {
      private InstructionHelperForAttributes helper = new InstructionHelperForAttributes();

      public string Id
      { get { return "AddStructuredDocs"; } }

      public bool DoInstruction(IDom part,
                   MetadataContextStack contextStack,
                   List<IDom> retList,
                   ref IDom lastPart)
      {
         // always return false, even if this does somethign, it doesn't take care of the underlying part
         var candidates = helper.GetMatchingAttributes(part, Id);
         foreach (var attribute in candidates)
         {
            if (HandleAttribute(attribute, part as IHasAttributes, contextStack, retList, ref lastPart)) return false;
         }
         return false;
      }

      private bool HandleAttribute(IAttribute attribute, IHasAttributes item, MetadataContextStack contextStack, List<IDom> newList, ref IDom lastPart )
      {
         var value = helper.GetBestMetadata<CodeFirstMetadata>(attribute, contextStack);
         var targetHasStructuredDocs = item as IHasStructuredDocumentation;
         if (value != null && targetHasStructuredDocs != null && !string.IsNullOrWhiteSpace(value.XmlCommentString ))
         {
           // targetHasStructuredDocs.StructuredDocumentation = value.XmlCommentString ;
         }
         return false;
      }
   }
}
