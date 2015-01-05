using CodeFirst.Common;
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
      public static ExpansionFirstTemplate<TMetadata> LoadFromFile<TMetadata>(string fileName)
       where TMetadata : CodeFirstMetadata<TMetadata>
      {
         var newTemplate = new ExpansionFirstTemplate<TMetadata>(RDom.CSharp, RDom.CSharp.LoadFromFile(fileName));
         return newTemplate;
      }

      public static ExpansionFirstTemplate<TMetadata> Load<TMetadata>(string code)
      where TMetadata : CodeFirstMetadata<TMetadata>
      {
         var newTemplate = new ExpansionFirstTemplate<TMetadata>(RDom.CSharp, RDom.CSharp.Load(code));
         return newTemplate;
      }
   }
}
