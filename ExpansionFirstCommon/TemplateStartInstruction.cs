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

   public class TemplateStartInstruction : IInstruction
   {

      private InstructionHelper helper = new InstructionHelper();
      private Regex blockMatch = new Regex( @"_xf_TemplateStart()");

      public string Id
      { get { return "TemplateStart"; } }

      public bool DoInstruction(IDom part,
                   MetadataContextStack contextStack,
                   List<IDom> retList,
                   ref IDom lastPart,
                   ref bool reRootTemplate)
      {
         // TODO: Find a better way to remove the outer gunk than the current aftermarket hack
         //if (DoInstructionInternal(part as IDetailBlockStart, contextStack, retList, ref lastPart))
         //{
            //var root = part.Ancestors.OfType<IRoot>().First();
            //root.StemMembersAll.AddOrMoveRange(retList);
            //reRootTemplate = true;
         //   return true;
         //}
         return false;
      }


      private bool DoInstructionInternal(IDetailBlockStart blockStart,
                        MetadataContextStack contextStack,
                        List<IDom> newList,
                        ref IDom lastPart)
      {
         if (blockStart == null) return false;
         if (!blockMatch.IsMatch(blockStart.Text)) return false;

         var blockContents = blockStart.BlockContents;
         newList.Clear();
         helper.RunOneLoop(blockContents, blockStart, contextStack, newList);
         lastPart = blockStart.BlockEnd;
         return true;
      }
   }
}
