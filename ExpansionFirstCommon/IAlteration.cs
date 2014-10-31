using ExpansionFirstTemplates;
using RoslynDom.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpansionFirst.Common
{
   public interface IAlteration
   {
      string Id { get; }
      void DoAlteration(IDom item, MetadataContextStack contextStack);
   }
}
