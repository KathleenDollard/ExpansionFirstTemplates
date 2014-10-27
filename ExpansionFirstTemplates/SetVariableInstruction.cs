using ExpansionFirs.tCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoslynDom.Common;
using System.Text.RegularExpressions;

namespace ExpansionFirstTemplates
{

   public class SetVariableInstruction : IInstruction
   {
      private Regex pubilcAnnotationMatch = new Regex(@"_xf_SetVariable");
      private Regex symbolMatch = new Regex(@"_xfSet_(\w+)");

      public string Id
      { get { return "SetVariable"; } }

        public IEnumerable<IDom> DoInstruction(IDom part,
                     MetadataContextStack contextStack)
      {
         var ret = new List<IDom>();
         if (DoInstruction(part as IPublicAnnotation, contextStack, ret)) return ret;
         if (DoInstruction(part as IDeclarationStatement, contextStack, ret)) return ret;
         if (DoInstruction(part as IField, contextStack, ret)) return ret;
         return null;
      }

      private bool DoInstruction(IPublicAnnotation publicAnnotation,
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

      private bool DoInstruction(IDeclarationStatement declaraion,
                    MetadataContextStack contextStack,
                    IEnumerable<IDom> newList)
      {
         if (declaraion == null ) return false;
         var match = symbolMatch.Match(declaraion.Name);
         if (match == null) return false;
         var name = match.Value;
         // TODO: Allow evaluating expressions in the following
         var value = declaraion.Initializer.ToString(); 
         contextStack.Current.AddValue(name, value);
         return true;
      }

            private bool DoInstruction(IField field,
                          MetadataContextStack contextStack,
                          IEnumerable<IDom> newList)
      {
         if (field == null) return false;
         var match = symbolMatch.Match(field.Name);
         if (match == null) return false;
         var name = match.Value;
         // TODO: Allow evaluating expressions in the following
         var value = field.Initializer.ToString();
         contextStack.Current.AddValue(name, value);
         return true;
      }
   }
}
