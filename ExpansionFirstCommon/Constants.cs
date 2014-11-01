using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpansionFirst.Common
{
   public class Constants
   {
      public const string ExpansionFirstRunner = "ExpansionFirst";
      public const string IsInOutsideTemplateRunner = "IsInOutsideTemplate";
      public const string Metadata = "Meta"; // This is widely used in templates and should not be changed. If you hate the semantics, add another. 

      public const string FormatCamelCase = "CamelCase";
      public const string FormatUpperCase = "Upper";
      public const string FormatLowerCase = "Lower";
      public const string FormatPascalCase = "PascalCase";
   }
}
