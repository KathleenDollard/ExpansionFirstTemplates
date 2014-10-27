using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoslynDom;
using RoslynDom.Common;
using RoslynDom.CSharp;
using System.Text.RegularExpressions;
using ExpansionFirs.tCommon;

namespace ExpansionFirstTemplates
{
   public class ExpansionFirstTemplate
   {
      private IRoot templateRoot;
      private List<IInstruction> availableInstructions = new List<IInstruction>();

      private ExpansionFirstTemplate() { } // Block instantiation
      public static ExpansionFirstTemplate LoadFromFile(string fileName)
      {
         var newTemplate = new ExpansionFirstTemplate();
         newTemplate.templateRoot = RDom.CSharp.LoadFromFile(fileName);
         return newTemplate;
      }

      public static ExpansionFirstTemplate Load(string code)
      {
         var newTemplate = new ExpansionFirstTemplate();
         newTemplate.templateRoot = RDom.CSharp.Load(code);
         return newTemplate;
      }

      private void Initialize()
      {
         availableInstructions.Add(new SetVariableInstruction());
      }

      public IEnumerable<IRoot> Run<TMetadata>(TMetadata metadata)
      {
         Initialize();
         var metaContextStack = new MetadataContextStack();
         metaContextStack.Push("Meta", metadata);
         return InternalRun(templateRoot, metaContextStack).Cast<IRoot>();
         // don't bother popping the stack, because we're done and at the top
      }

         public IEnumerable<IDom> InternalRun<T>(T part, MetadataContextStack contextStack)
             where T : IDom
      {
         var type = part.GetType();
         var iDomInterface = type.GetInterfaces()
                              .Where(x => x.Name == "IDom`1")
                              .FirstOrDefault();
         if (iDomInterface == null) throw new InvalidOperationException();
         var iType = iDomInterface.GenericTypeArguments.First();
         var method = ReflectionUtilities.MakeGenericMethod(this.GetType(), "InternalRunAsT", iType);
         return  method.Invoke(this, new object[] { part, contextStack }) as IEnumerable<IDom>;
      }

      public IEnumerable<IDom> InternalRunAsT<T>(T part, MetadataContextStack contextStack)
             where T : IDom<T>
      {
         var ret = new List<T>();
         var result = DoInstruction(part, contextStack);
         if (result != null) return result;

         var newItem = part.Copy();
         DoReplacements(newItem, contextStack);

         var partAsContainer = newItem as IContainer;
         if (partAsContainer != null)
         {
            foreach (var member in partAsContainer.GetMembers()) { InternalRun(member, contextStack); }
         }

         return new IDom[] { newItem };
      }

      private IEnumerable<IDom> DoInstruction<T>(T part, MetadataContextStack contextStack)
          where T : IDom
      {
         var publicAnnotation = part as IPublicAnnotation;
         if (publicAnnotation == null) return null;
         foreach (var instruction in availableInstructions)
         {
            var result = instruction.DoInstruction(publicAnnotation, contextStack);
            if (result != null) return result;
         }
         return null;
      }

      private void DoReplacements<T>(T newItem, MetadataContextStack metaContextStack)
               where T : IDom
      {
         // TODO: Do Replacements
      }
   }
}
