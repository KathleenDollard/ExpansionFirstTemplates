using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpansionFirst.Support
{
   public class _xf_
   {
      public class OutputXmlComments : Attribute
      { }

      public class OutputAttributes : Attribute
      { }

      public class OutputAsPartialAttribute : Attribute
      {
         public OutputAsPartialAttribute()
         { }
         public OutputAsPartialAttribute(bool outputPartial)
         { OutputPartial = outputPartial; }

         public bool OutputPartial { get; private set; }
      }

   }
}
