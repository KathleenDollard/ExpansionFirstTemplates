using RoslynDom.Common;
using System.Collections.Generic;

namespace ExpansionFirst.Common
{
   public interface IInstruction
   {
      string Id { get; }
      bool DoInstruction(IDom part, MetadataContextStack contextStack, List<IDom> retList, ref IDom lastPart);
   }
}
