using System.Collections.Generic;
using RoslynDom.Common;
using System;

namespace ExpansionFirst.Common
{

   public partial class _xf_
   {
      public class AttributesAttribute : Attribute
      {
         public AttributesAttribute()
         { }
         public AttributesAttribute(string sourceName)
         { SourceName = sourceName; }

         public string SourceName { get; private set; }
      }
   }

   public class AttributesInstruction : InstructionBase
   {
      private const string id = "Attributes";

      public AttributesInstruction() : base(id) { }

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
         if (item == null) return false;
         var value = Helper.GetBestMetadata<IHasAttributes>(attribute, contextStack);
         if (value != null)
         {
            item.Attributes.AddOrMoveAttributeRange(value.Attributes);
         }
         return false;
      }
   }
}
