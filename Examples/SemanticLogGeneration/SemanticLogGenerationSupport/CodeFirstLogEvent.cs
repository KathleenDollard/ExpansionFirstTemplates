﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Tracing;
using System.Reflection;
using CodeFirst.Common;

namespace CodeFirstMetadataTest.SemanticLog
{

    public class CodeFirstLogEvent : CodeFirstLogEventBase
    {
        protected override bool ValidateAndUpdateCore()
        {
            var isOk = base.ValidateAndUpdateCore();
            if (isOk)
            { return EventId > 0; }
            return false;
        }

    }
}
