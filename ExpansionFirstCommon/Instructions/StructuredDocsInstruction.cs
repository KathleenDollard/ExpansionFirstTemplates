using System.Collections.Generic;
using RoslynDom.Common;
using System;
using CodeFirst.Common;

namespace ExpansionFirst.Common
{

   public partial class _xf_
   {
      public class StructuredDocsAttribute : Attribute
      {
         public StructuredDocsAttribute()
         { }
         public StructuredDocsAttribute(string sourceName)
         { SourceName = sourceName; }

         public string SourceName { get; private set; }
      }
   }

   public class StructuredDocsInstruction : InstructionBase 
   {
      private const string id = "StructuredDocs";

      public StructuredDocsInstruction() : base(id) { }

      public override bool BeforeCopy(IDom sharedPart, 
                  MetadataContextStack contextStack, 
                  List<IDom> retList, 
                  ref IDom lastPart)
      {
         // always return false, even if this does somethign, it doesn't take care of the underlying part
         var candidates = Helper.GetMatchingAttributes(sharedPart, Id);
         foreach (var attribute in candidates)
         {
            if (HandleAttribute(attribute, sharedPart as IHasAttributes, contextStack, retList, ref lastPart)) return false;
         }
         return false;
      }

      private bool HandleAttribute(IAttribute attribute, IHasAttributes item, MetadataContextStack contextStack, List<IDom> newList, ref IDom lastPart)
      {
         var value = Helper.GetBestMetadata<CodeFirstMetadata>(attribute, contextStack);
         var targetHasStructuredDocs = item as IHasStructuredDocumentation;
         if (value != null && targetHasStructuredDocs != null && !string.IsNullOrWhiteSpace(value.XmlCommentString))
         {
            // targetHasStructuredDocs.StructuredDocumentation = value.XmlCommentString ;
         }
         return false;
      }
   }
}
