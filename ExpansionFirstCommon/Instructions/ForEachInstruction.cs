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

   public class ForEachInstruction : InstructionBase
   {
      private static Regex blockStartMatch;
      private const string loopOverKey = "LoopOver";
      private const string varNameKey = "VarName";
      private const string id = "ForEach";

      public ForEachInstruction() : base(id) { }

      public override bool BeforeCopy(IDom sharedPart, 
                  MetadataContextStack contextStack, 
                  List<IDom> retList, 
                  ref IDom lastPart)
      {
         blockStartMatch = Helper.BuildSpecificParamRegex(id, loopOverKey, varNameKey);
         if (DoInstructionInternal(sharedPart as IDetailBlockStart, contextStack, retList, ref lastPart)) return true;
         return false;
      }

         private bool DoInstructionInternal(IDetailBlockStart blockStart,
                        MetadataContextStack contextStack,
                        List<IDom> newList,
                        ref IDom lastPart)
      {
         if (blockStart == null) return false;
         var match = blockStartMatch.Match(blockStart.Text);
         if (!match.Success) return false;

         var varName = match.Groups[varNameKey].Value;
         var loopOver = match.Groups[loopOverKey].Value;
         IEnumerable propAsIEnumerable = GetPropList(contextStack, loopOver);
         var blockContents = blockStart.BlockContents;
         foreach (var item in propAsIEnumerable)
         {
            contextStack.Push(varName, item);
            Helper.RunOneLoop(blockContents, blockStart, contextStack, newList);
            contextStack.Pop();
         }
         lastPart = blockStart.BlockEnd;
         return true;
      }

      private static IEnumerable GetPropList(MetadataContextStack contextStack, string loopOver)
      {
         // TODO: Work out multi-part naming
         var metaVar = loopOver.SubstringBefore(".");
         var metaProp = loopOver.SubstringAfter(".");
         var metaVarValue = contextStack.GetValue(metaVar);
         var metaPropValue = metaVarValue.GetType().GetProperty(metaProp).GetValue(metaVarValue);
         var propAsIEnumerable = metaPropValue as IEnumerable;
         return propAsIEnumerable;
      }
   }
}
