using ExpansionFirstTemplates;
using RoslynDom.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpansionFirs.tCommon
{
   public interface IInstruction
   {
      string Id { get; }
      IEnumerable<IDom> DoInstruction(IDom part, MetadataContextStack contextStack);
   }
}
