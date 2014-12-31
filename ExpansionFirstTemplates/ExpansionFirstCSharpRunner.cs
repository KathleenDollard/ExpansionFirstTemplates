using System;
using CodeFirst.Common;
using CodeFirst.TemplateSupport;
using System.Linq;
using RoslynDom;
using ExpansionFirst.Common;
using RoslynDom.CSharp;

namespace ExpansionFirstTemplates
{
   public class ExpansionFirstCSharpRunner : TemplateRunnerBase
   {

      public ExpansionFirstCSharpRunner(ICodeFirstServiceProvider serviceProvider) : base(serviceProvider) { }

      public ExpansionFirstCSharpRunner() : base(null) { }

      public override string CreateString<TMetadata, TTemplate>(TMetadata metadata, TTemplate template)
      {
         // TODO: Loosen the typing here as much as practical, interface?
         var xfTemplate = template as ExpansionFirstTemplate;
         if (xfTemplate == null) throw new InvalidOperationException("Template must be of type ExpansionFirstTemplate");
         var newRoots = xfTemplate.Run(metadata);
         var newRDomRoot = newRoots.First() as RDomRoot;
         var output= RDom.CSharp.GetFormattedSourceCode(newRDomRoot);
         return output;
      }
   }

}