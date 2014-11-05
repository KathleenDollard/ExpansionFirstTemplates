using ExpansionFirst.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoslynDom.Common;

namespace ExpansionFirst.Common
{
   public abstract class InstructionBase : IInstruction
   {
      private string id;
      // TODO: Dude, this should be retrieved via DI, maybe or maybe not. Stuck on the interface and all that jazz? for goodness sakes can we look at that later 
      private InstructionHelper helper = new InstructionHelper();

      protected InstructionBase(string id)
      { this.id = id; }

      public string Id
      { get { return id; } }

      public virtual void RunStart(MetadataContextStack contextStack) { }
      public virtual void RunEnd(MetadataContextStack contextStack) { }

      public virtual void TemplateStart(IRoot sharedRoot, MetadataContextStack contextStack) { }
      public virtual void TemplateEnd(IRoot newRoot, MetadataContextStack contextStack) { }

      public virtual bool BeforeCopy(IDom sharedPart, MetadataContextStack contextStack, List<IDom> retList, ref IDom lastPart)
      { return false; }
      public virtual void AfterCopy(IDom newPart, MetadataContextStack contextStack, List<IDom> retList) { }

      protected InstructionHelper Helper
      { get { return helper; } }
   }
}
