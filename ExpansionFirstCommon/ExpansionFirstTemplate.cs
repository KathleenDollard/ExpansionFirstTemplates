using System;
using System.Collections.Generic;
using System.Linq;
using RoslynDom.Common;
using ExpansionFirst.Common;
using ExpansionFirst;

namespace ExpansionFirst.Common
{
   public class ExpansionFirstTemplate
   {
      private IRoot templateRoot;
      private List<IInstruction> availableInstructions = new List<IInstruction>();
      private ReplacementAlteration replacementAlteration = new ReplacementAlteration();

      public ExpansionFirstTemplate(IRoot templateRoot)
      {
         this.templateRoot = templateRoot;
      }

      private void Initialize()
      {
         availableInstructions.Add(new SetVariableInstruction());
         availableInstructions.Add(new ForEachInstruction());
         availableInstructions.Add(new AddStructuredDocsInstruction());
         availableInstructions.Add(new AttributesInstruction());
         availableInstructions.Add(new TemplateStartInstruction());
      }

      public IEnumerable<IRoot> Run<TMetadata>(TMetadata metadata)
      {
         Initialize();
         var contextStack = new MetadataContextStack();
         contextStack.Push(Constants.Metadata, metadata);
         contextStack.Add(Constants.IsInOutsideTemplateRunner, true);
         contextStack.Add(Constants.ExpansionFirstRunner, this);
         IDom nextPart = null;
         return Update(templateRoot, contextStack, ref nextPart).Cast<IRoot>();
         // don't bother popping the metadata stack, because we're done and at the top
      }

      internal IEnumerable<IDom> Update(IDom part, MetadataContextStack contextStack, ref IDom lastPart)
      {
         var newMemberList = new List<IDom>();

         var method = part.GetType().GetMethod("Copy");
         var newItem = method.Invoke(part, null) as IDom;

         bool reRoot = false;
         if (DoInstruction(part, contextStack, newMemberList, ref lastPart, ref reRoot))
         {
            return newMemberList.OfType<IDom>();
         }

         DoReplacements(newItem, contextStack);
         HandleAttributes(newItem as IHasAttributes, contextStack);

         UpdateProperty(contextStack, newItem as IProperty);
         UpdateContainer(contextStack, newMemberList, newItem as IContainer);

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

      private void UpdateContainer(MetadataContextStack contextStack, List<IDom> newMemberList, IContainer newContainer)
      {
         if (newContainer != null)
         {
            var member = newContainer.GetMembers().FirstOrDefault();
            var i = 0;
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



      //public IEnumerable<IDom> InternalRun<T>(T part, MetadataContextStack contextStack, ref IDom nextPart)
      //    where T : IDom
      //{
      //   var type = part.GetType();
      //   var iDomInterface = type.GetInterfaces()
      //                        .Where(x => x.Name == "IDom`1")
      //                        .FirstOrDefault();
      //   if (iDomInterface == null) throw new InvalidOperationException();
      //   var iType = iDomInterface.GenericTypeArguments.First();
      //   var method = ReflectionUtilities.MakeGenericMethod(this.GetType(), "InternalRunAsT", iType);
      //   return method.Invoke(this, new object[] { part, contextStack }) as IEnumerable<IDom>;
      //}

      //public IEnumerable<IDom> InternalRunAsT<T>(T part, MetadataContextStack contextStack, ref IDom nextPart)
      //         where T : IDom<T>
      //{
      //   var ret = new List<T>();

      //   if (DoInstruction(part, contextStack, ret, ref nextPart)) return ret.OfType<IDom>();

      //   //if (HandleBlock(part, contextStack, ret, ref nextPart)) return ret.OfType<IDom>();

      //   var newItem = part.Copy();
      //   DoReplacements(newItem, contextStack);

      //   var newPartAsContainer = newItem as IContainer;
      //   if (newPartAsContainer != null)
      //   {
      //      var members = newPartAsContainer.GetMembers();
      //      var member = members.FirstOrDefault();
      //      var i = 0;
      //      while (member != null)
      //      {
      //         i++; if (i > 1000) throw new InvalidOperationException("Infinite loop detected");
      //         var nextMember = members.FollowingSiblings(member).FirstOrDefault();
      //         var newMembers = InternalRun(member, contextStack, ref nextMember);
      //         newPartAsContainer.RemoveMember(member);
      //         foreach (var newMember in newMembers)
      //         { newPartAsContainer.AddOrMoveMember(newMember); }
      //      }
      //   }

      //   return new IDom[] { newItem };
      //}

      //public IEnumerable<IDom> InternalRunAsT<T>(T part, MetadataContextStack contextStack)
      //       where T : IDom<T>
      //{
      //   var ret = new List<T>();
      //   var result = DoInstruction(part, contextStack);
      //   if (result != null) return result;

      //   var newItem = part.Copy();
      //   DoReplacements(newItem, contextStack);

      //   var partAsContainer = newItem as IContainer;
      //   if (partAsContainer != null)
      //   {
      //      foreach (var member in partAsContainer.GetMembers())
      //      {
      //         var newMembers = InternalRun(member, contextStack);
      //         partAsContainer.RemoveMember(member);
      //         foreach (var newMember in newMembers)
      //         { partAsContainer.AddOrMoveMember(newMember); }
      //      }
      //   }

      //   return new IDom[] { newItem };
      //}

      //private IEnumerable<IDom> DoInstruction<T>(T part, MetadataContextStack contextStack)
      //    where T : IDom
      //{
      //   var publicAnnotation = part as IPublicAnnotation;
      //   if (publicAnnotation == null) return null;
      //   foreach (var instruction in availableInstructions)
      //   {
      //      var result = instruction.DoInstruction(publicAnnotation, contextStack);
      //      if (result != null) return result;
      //   }
      //   return null;
      //}


      private bool DoInstruction(IDom part, MetadataContextStack contextStack, List<IDom> retList,
                                    ref IDom lastPart, ref bool reRootTemplate)
      {
         //var publicAnnotation = part as IPublicAnnotation;
         //if (publicAnnotation == null) return false;
         foreach (var instruction in availableInstructions)
         {
            bool reRoot = false;
            if (instruction.BeforeCopy (part, contextStack, retList, ref lastPart))
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
   }
}
