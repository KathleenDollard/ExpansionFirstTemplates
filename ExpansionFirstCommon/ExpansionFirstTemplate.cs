using System;
using System.Collections.Generic;
using System.Linq;
using RoslynDom.Common;
using ExpansionFirst.Common;
using ExpansionFirst;
using CodeFirst.Common;

namespace ExpansionFirst.Common
{
   public class ExpansionFirstTemplate
   {
      private IRoot templateRoot;
      private List<IInstruction> availableInstructions = new List<IInstruction>();
      private ReplacementAlteration replacementAlteration = new ReplacementAlteration();
      private MetadataContextStack contextStack = new MetadataContextStack();
      private InstructionHelper helper = new InstructionHelper();
      private string filePathHint;
      protected IFactoryAccess factoryAccess;

      /// <summary>
      /// 
      /// </summary>
      /// <param name="templateRoot"></param>
      /// <remarks>
      /// Currently each expnasion first class wraps a single template and templates are 
      /// naked RoslynDom trees. Expect this to change because it templates need conditionals
      /// attached, and it's not clear that this runner should interface, surface decisions
      /// regarding that conditionals. Also, it should not be up to an external client
      /// to loop through multiple metadata sets - each call to run is one metadata set. 
      /// And in case you weren't already convinced this is a bad place for a complex 
      /// dependency, someone needs to manage file boundaries for file based geenration and
      /// fragment based for fragment based generation. 
      /// </remarks>
      public ExpansionFirstTemplate(IFactoryAccess factoryAccess, IRoot templateRoot)
      {
         this.factoryAccess = factoryAccess;
         this.templateRoot = templateRoot;
         var filePathHint = templateRoot.Descendants
                              .OfType<IPublicAnnotation>()
                              .Where(x => x.Name == "_xf_FilePathHint")
                              .FirstOrDefault();
         if (filePathHint != null)
         {
            this.filePathHint = filePathHint.GetValue("").ToString();
         }

         Initialize();
      }

      private void Initialize()
      {
         // TODO: Instructions will be DI. Probably, a bootstrapper instruction here will set up the container, because I want complete independence from who/where/why this system/DLL is used
         // TODO: There si a set of expected instructions, and a priority system to override these is needed. Possibly based on ID and a well known enum creating id strings
         availableInstructions.Add(new SetVariableInstruction());
         availableInstructions.Add(new ForEachInstruction());
         availableInstructions.Add(new StructuredDocsInstruction());
         availableInstructions.Add(new AttributesInstruction());
         availableInstructions.Add(new TemplateStartInstruction());

         DoRunInitialize();
      }

      public IEnumerable<IRoot> Run<TMetadata>(TMetadata metadata)
      {
         contextStack.Push(Constants.Metadata, metadata);
         contextStack.Add(Constants.ExpansionFirstRunner, this);
         var thisRoot = templateRoot.Copy();
         DoTemplateStart(thisRoot);
         IDom nextPart = null;
         var retList = Update(thisRoot, contextStack, ref nextPart).Cast<IRoot>();
         DoTemplateComplete(thisRoot, retList);
         contextStack.Pop();
         return retList;
      }

      public string FilePathHint
      { get { return this.filePathHint; } }

      public string Name
      { get { return templateRoot.FilePath; } }

      public void RunComplete()
      { DoSomeInstruction((ins, cs) => ins.RunComplete(cs)); }

      internal IEnumerable<IDom> Update(IDom part, MetadataContextStack contextStack, ref IDom lastPart)
      {
         var newItem = helper.Copy<IDom>(part);

         // Returning true from DoInstruction means the part is fully handled and no more should be done
         {
            var newMemberList = new List<IDom>();
            if (DoInstruction(part, contextStack, newMemberList, ref lastPart))
            {
               return newMemberList.OfType<IDom>();
            }
         }

         // TODO: I'm not convinced there are three actions, it may be 3 instructions. 
         DoReplacements(newItem, contextStack);
         DoAfterCopy(newItem);
         HandleAttributes(newItem as IHasAttributes, contextStack);

         // Manage children. Property has a special case of multiple children. Events may be the same. 
         UpdateProperty(contextStack, newItem as IProperty);
         UpdateContainer(contextStack, newItem as IContainer);

         return new IDom[] { newItem };
      }

      private void HandleAttributes(IHasAttributes newItemHasAttributes, MetadataContextStack contextStack)
      {
         if (newItemHasAttributes == null) return;
         var attributes = newItemHasAttributes.Attributes;
         var xfAttributes = attributes.Where(x => x.Name.StartsWith("_xf_.")).ToList();
         foreach (var attribute in xfAttributes)
         {
            var name = attribute.Name.SubstringAfter("_xf_.");
         }
      }

      private void UpdateContainer(MetadataContextStack contextStack, IContainer newContainer)
      {
         if (newContainer != null)
         {
            var member = newContainer.GetMembers().FirstOrDefault();
            var i = 0;
            var newMemberList = new List<IDom>();
            while (member != null)
            {
               i++; if (i > 1000) throw new InvalidOperationException("Infinite loop detected");
               var lastMember = member;
               newMemberList.AddRange(Update(member, contextStack, ref lastMember));
               var nextMember = lastMember.NextSibling();
               member = nextMember;
            }
            UpdateItem(newContainer, newMemberList);
         }
      }

      private void UpdateProperty(MetadataContextStack contextStack, IProperty newProperty)
      {
         if (newProperty != null)
         {
            // yes, ugly hack
            IDom dummy = null;
            if (newProperty.GetAccessor != null)
            { newProperty.GetAccessor = Update(newProperty.GetAccessor, contextStack, ref dummy).First() as IAccessor; }
            if (newProperty.SetAccessor != null)
            { newProperty.SetAccessor = Update(newProperty.SetAccessor, contextStack, ref dummy).First() as IAccessor; }
         }
      }

      private void UpdateItem(IContainer newPartAsContainer, List<IDom> ret)
      {
         foreach (var mmeber in newPartAsContainer.GetMembers())
         { newPartAsContainer.RemoveMember(mmeber); }
         foreach (var newMember in ret)
         { newPartAsContainer.AddOrMoveMember(newMember); }
      }

      private bool DoInstruction(IDom part, MetadataContextStack contextStack, List<IDom> retList,
                                    ref IDom lastPart)
      {
         // Can't use DoSomeInstruction becuase you can't pass ref to a lambda
         foreach (var instruction in availableInstructions)
         {
            if (instruction.BeforeCopy(part, contextStack, retList, ref lastPart))
            { return true; }
         }
         return false;
      }

      private void DoReplacements<T>(T newItem, MetadataContextStack metaContextStack)
               where T : IDom
      {
         replacementAlteration.DoAlteration(newItem, metaContextStack);
         // TODO: Do Replacements
      }

      private void DoRunInitialize()
      { DoSomeInstruction((ins, cs) => ins.RunInitialize(cs)); }

      private void DoTemplateStart(IRoot sharedRoot)
      { DoSomeInstruction((ins, cs) => ins.TemplateStart(sharedRoot, cs)); }

      private void DoTemplateComplete(IRoot sharedRoot, IEnumerable<IRoot> retList)
      { DoSomeInstruction((ins, cs) => ins.TemplateDone(sharedRoot, cs, retList)); }

      private void DoAfterCopy(IDom newPart)
      { DoSomeInstruction((ins, cs) => ins.AfterCopy(newPart, cs)); }

      private void DoSomeInstruction(Action<IInstruction, MetadataContextStack> instructionPartDelegate)
      {
         foreach (var instruction in availableInstructions)
         {
            instructionPartDelegate(instruction, contextStack);
         }
      }
   }
   public class ExpansionFirstTemplate<T> : ExpansionFirstTemplate, ITemplate<T>
      where T :CodeFirstMetadata<T>
   {
      public ExpansionFirstTemplate(IFactoryAccess factoryAccess, IRoot templateRoot) : base(factoryAccess, templateRoot)
      { }
      public Type MetadataType
      { get { return typeof(T); } }

      public string GetOutput(T metadata)
      {
         var ret = "";
         var roots = Run(metadata);
         foreach (var root in roots)
         {
            var output= factoryAccess.GetFormattedSourceCode(root);
            ret += output;
         }
         return ret;
      }
   }
}
