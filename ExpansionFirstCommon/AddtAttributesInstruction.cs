using System.Collections.Generic;
using RoslynDom.Common;
using System;

namespace ExpansionFirst.Common
{

   public partial class _xf_
      {
         public class AddtAttributesAttribute : Attribute
         {
            public AddtAttributesAttribute()
            { }
            public AddtAttributesAttribute(string sourceName)
            { SourceName = sourceName; }

            public string SourceName { get; private set; }
         }
      }

   public class AddAttributesInstruction : IInstruction
   {
      private InstructionHelper helper = new InstructionHelper();

      public string Id
      { get { return "AddAttributes"; } }

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
         if (item == null) return false;
         var value = helper.GetBestMetadata<IHasAttributes>(attribute, contextStack);
         if (value != null)
         {
            item.Attributes.AddOrMoveAttributeRange(value.Attributes);
         }
         return false;
      }
   }
}
