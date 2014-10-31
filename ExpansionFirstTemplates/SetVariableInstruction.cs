using System.Collections.Generic;
using RoslynDom.Common;
using System.Text.RegularExpressions;
using ExpansionFirst.Common;

namespace ExpansionFirstTemplates
{

   public class SetVariableInstruction : IInstruction
   {
      private Regex pubilcAnnotationMatch = new Regex(@"_xf_SetVariable");
      private Regex symbolMatch = new Regex(@"_xf_Set_(\w+)");

      public string Id
      { get { return "SetVariable"; } }

      public bool DoInstruction(IDom part,
                   MetadataContextStack contextStack,
                   List<IDom> retList,
                   ref IDom lastPart)
      {
         var ret = new List<IDom>();
         if (DoInstructionInternal(part as IPublicAnnotation, contextStack, ret)) return true;
         if (DoInstructionInternal(part as IDeclarationStatement, contextStack, ret)) return true;
         if (DoInstructionInternal(part as IField, contextStack, ret)) return true;
         return false;
      }

      private bool DoInstructionInternal(IPublicAnnotation publicAnnotation,
                        MetadataContextStack contextStack,
                        IEnumerable<IDom> newList)
      {
         if (publicAnnotation == null || !(pubilcAnnotationMatch.IsMatch(publicAnnotation.Name))) return false;
         foreach (var key in publicAnnotation.Keys)
         {
            var value = publicAnnotation.GetValue(key);
            contextStack.Current.AddValue(key, value);
         }
         return true;
      }

      private bool DoInstructionInternal(IDeclarationStatement declaraion,
                       MetadataContextStack contextStack,
                        IEnumerable<IDom> newList)
      {
         if (declaraion == null) return false;
         var match = symbolMatch.Match(declaraion.Name);
         if (!match .Success) return false;
         var name = match.Value;
         // TODO: Allow evaluating expressions in the following
         var value = declaraion.Initializer.ToString();
         contextStack.Current.AddValue(name, value);
         return true;
      }

      private bool DoInstructionInternal(IField field,
                  MetadataContextStack contextStack,
                  IEnumerable<IDom> newList)
      {
         if (field == null) return false;
         var match = symbolMatch.Match(field.Name);
         if (!match .Success) return false;
         var name = match.Value;
         if (field.Initializer != null)
         {
            // TODO: Allow evaluating expressions in the following
            var value = field.Initializer.ToString();
            contextStack.Current.AddValue(name, value);
         }
         return true;
      }
   }
}
