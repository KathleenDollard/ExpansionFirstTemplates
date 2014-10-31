﻿using ExpansionFirstTemplates;
using RoslynDom.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpansionFirst.Common
{
   public interface IInstruction
   {
      string Id { get; }
      bool DoInstruction(IDom part, MetadataContextStack contextStack, List<IDom> retList, ref IDom lastPart);
   }
}
