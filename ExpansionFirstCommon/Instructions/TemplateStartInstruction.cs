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

      public override void TemplateDone(IRoot sharedRoot, MetadataContextStack contextStack,
                  IEnumerable<IRoot> retList)
      {
         foreach (var newRoot in retList)
         {
            var blockStart = newRoot
                              .Descendants
                              .OfType<IDetailBlockStart>()
                              .Where(x => x.Text.Contains(Helper.MakeMarker(id)))
                              .FirstOrDefault();
            if (blockStart == null) return;

            var blockContents = blockStart.BlockContents;
            var blockContentsAsStemMembers = blockContents
                           .OfType<IStemMemberAndDetail>()
                           .Select(x => Helper.Copy<IDom>(x));
            if (blockContents.Count() != blockContentsAsStemMembers.Count()) throw new InvalidOperationException();
            newRoot.StemMembersAll.Clear();
            newRoot.StemMembersAll
                  .AddOrMoveRange(blockContentsAsStemMembers.OfType<IStemMemberAndDetail>());
         }
      }
   }
}
