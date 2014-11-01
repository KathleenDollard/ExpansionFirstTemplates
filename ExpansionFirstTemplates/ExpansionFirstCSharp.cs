using ExpansionFirst.Common;
using RoslynDom.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpansionFirstTemplates
{
   public class ExpansionFirstCSharp
   {
      public static ExpansionFirstTemplate LoadFromFile(string fileName)
      {
         var newTemplate = new ExpansionFirstTemplate( RDom.CSharp.LoadFromFile(fileName));
         return newTemplate;
      }

      public static ExpansionFirstTemplate Load(string code)
      {
         var newTemplate = new ExpansionFirstTemplate(RDom.CSharp.Load(code));
         return newTemplate;
      }
  }
}
