using System.Collections.Generic;
using RoslynDom.Common;
using System.Text.RegularExpressions;
using ExpansionFirst.Common;

namespace ExpansionFirst.Common
{

   public class SetVariableInstruction : InstructionBase 
   {
      private const string id = "SetVariable";
      private string matchString = Constants.Prefix + id;

      public SetVariableInstruction() : base(id) { }

      public override bool BeforeCopy(IDom sharedPart, 
                  MetadataContextStack contextStack, 
                  List<IDom> retList, 
                  ref IDom lastPart)
      {
         var ret = new List<IDom>(); // block changes to the passed list
         if (DoInstructionInternal(sharedPart as IPublicAnnotation, contextStack, ret)) return true;
         if (DoInstructionInternal(sharedPart as IDeclarationStatement, contextStack, ret)) return true;
         if (DoInstructionInternal(sharedPart as IField, contextStack, ret)) return true;
         return false;
      }

      private bool NameMatches(string candidate, bool fullName = false)
      {
         if (fullName) return candidate == matchString;
         return candidate.StartsWith(matchString);
      }

      private void PushToContext(MetadataContextStack contextStack, string key,object value, bool fullName = false)
      {
         if (!fullName) key = key.SubstringAfter(matchString + "_");
         contextStack.Add(key, value);
      }

      private bool DoInstructionInternal(IPublicAnnotation publicAnnotation,
                        MetadataContextStack contextStack,
                        IEnumerable<IDom> newList)
      {
         if (publicAnnotation == null) return false;
         if (!NameMatches(publicAnnotation.Name, true)) return false;
         foreach (var key in publicAnnotation.Keys)
         {
            var value = publicAnnotation.GetValue(key);
            PushToContext(contextStack, key, value, true);
            //contextStack.Current.AddValue(key, value);
         }
         return true;
      }

      private bool DoInstructionInternal(IDeclarationStatement declaration,
                       MetadataContextStack contextStack,
                        IEnumerable<IDom> newList)
      {
         if (declaration == null) return false;
         if (!NameMatches(declaration.Name)) return false;
         var name = declaration.Name;
         object value = null;
         if (declaration.Initializer != null)
         {
            // TODO: Allow evaluating expressions in the following
            value = declaration.Initializer.Expression.ToString();
         }
         PushToContext(contextStack, name, value);
         //contextStack.Current.AddValue(name, value);
         return true;
      }

      private bool DoInstructionInternal(IField field,
                  MetadataContextStack contextStack,
                  IEnumerable<IDom> newList)
      {
         if (field == null) return false;
         if (!NameMatches(field.Name)) return false;
         var name = field.Name;
         object value = null;
         if (field.Initializer != null)
         {
            // TODO: Allow evaluating expressions in the following
            value = field.Initializer.Expression.ToString();
         }
         PushToContext(contextStack, name, value);
         //contextStack.Current.AddValue(name, value);
         return true;
      }
   }
}
