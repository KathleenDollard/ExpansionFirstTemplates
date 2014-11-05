using System.Collections.Generic;
using RoslynDom.Common;
using RoslynDom.RoslynDomUtilities;
using System.Text.RegularExpressions;
using ExpansionFirst.Common;
using System.Linq;
using System.Collections;
using RoslynDom;
using System;

namespace ExpansionFirst.Common
{

   public class TemplateStartInstruction : InstructionBase
   {
      private const string id = "TemplateStart";

      public TemplateStartInstruction() : base(id) { }

      public override void TemplateEnd(IRoot newRoot, MetadataContextStack contextStack)
      {
         var blockStart = newRoot
                           .Descendants
                           .OfType<IDetailBlockStart>()
                           .Where(x => x.Text.Contains(Helper.MakeMarker(id)))
                           .FirstOrDefault();
         if (blockStart == null) return ;

         var blockContents = blockStart.BlockContents;
         var blockContentsAsStemMembers = blockContents.OfType<IStemMemberAndDetail>();
         if (blockContents.Count() != blockContentsAsStemMembers.Count()) throw new InvalidOperationException();
         newRoot.StemMembersAll.Clear();
         newRoot.StemMembersAll.AddOrMoveRange(blockContentsAsStemMembers);
      }
   }
}
