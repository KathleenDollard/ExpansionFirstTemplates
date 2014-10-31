using System.Collections.Generic;
using RoslynDom.Common;
using RoslynDom.RoslynDomUtilities;
using System.Text.RegularExpressions;
using ExpansionFirst.Common;
using System.Linq;
using System.Collections;
using RoslynDom;
using System;

namespace ExpansionFirstTemplates
{

   public class ForEachInstruction : IInstruction
   {
      // TODO: Move param string to central location
      private const string paramString = @"\s*{0}\s*[=:]\s*\""(?<{0}>.*)\""\s*";
      private const string loopOverKey = "LoopOver";
      private const string varNameKey = "VarName";
      private static string loopOverRegex = string.Format(paramString, loopOverKey);
      private static string varNameRegex = string.Format(paramString, varNameKey);
      private static string regExString = string.Format(@"_xf_ForEach\s*\({0},{1}\)", loopOverRegex, varNameRegex);
      private static Regex blockStartMatch = new Regex(regExString);

      public string Id
      { get { return "ForEach"; } }

      public bool DoInstruction(IDom part,
                   MetadataContextStack contextStack,
                   List<IDom> retList,
                   ref IDom lastPart)
      {
         var ret = new List<IDom>();
         if (DoInstructionInternal(part as IDetailBlockStart, contextStack, ret, ref lastPart)) return true;
         return false;
      }

      private bool DoInstructionInternal(IDetailBlockStart blockStart,
                        MetadataContextStack contextStack,
                        List<IDom> newList,
                        ref IDom lastPart)
      {
         if (blockStart == null) return false;
         // if (!(blockStartMatch.IsMatch(blockStart.Text))) return false;
         var match = blockStartMatch.Match(blockStart.Text);
         if (!match.Success) return false;

         var varName = match.Groups[varNameKey].Value;
         IEnumerable propAsIEnumerable = GetEnumerable(contextStack, match);
         var expansionFirstRunner = contextStack.GetValue(Constants.ExpansionFirstRunner) as ExpansionFirstTemplate;
         var container = blockStart.Ancestors.OfType<IContainer>().First();
         var blockContents = blockStart.BlockContents;
         foreach (var item in propAsIEnumerable)
         {
            contextStack.Push(varName, item);
            var member = blockContents.FirstOrDefault();
            var i = 0;
            while (member != null)
            {
               i++; if (i > 1000) throw new InvalidOperationException("Infinite loop detected");
               var lastMember = member;
               var newMembers = expansionFirstRunner.InternalRun(member, contextStack, ref lastMember);
               var nextMember = lastMember.NextSibling();
               // don't store the end region that matches this block start, because it's being removed
               newMembers = newMembers
                              .Where(x =>
                              {
                                 var block = x as IDetailBlockEnd;
                                 if (block == null) return true;
                                 return (block.GroupGuid != blockStart.GroupGuid);
                              });
               newList.AddRange(newMembers);
               member = nextMember;
            }
            contextStack.Pop();
         }
         lastPart = blockStart.BlockEnd ;
         return true;
      }

      private static IEnumerable GetEnumerable(MetadataContextStack contextStack, Match match)
      {
         var loopOver = match.Groups[loopOverKey].Value;
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
