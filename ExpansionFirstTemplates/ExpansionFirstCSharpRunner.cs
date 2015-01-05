using System;
using CodeFirst.Common;
using CodeFirst.TemplateSupport;
using System.Linq;
using RoslynDom;
using ExpansionFirst.Common;
using RoslynDom.CSharp;
using System.Collections.Generic;
using System.IO;

namespace ExpansionFirstTemplates
{
   public class ExpansionFirstCSharpRunner<TMetadata> : TemplateRunnerBase
      where TMetadata :CodeFirstMetadata<TMetadata>
   {
      private IEnumerable<string> templateDirectoryNames;

      public ExpansionFirstCSharpRunner(ICodeFirstServiceProvider serviceProvider,  params string[] templateDirectoryNames) : base(serviceProvider)
      {
         this.templateDirectoryNames = templateDirectoryNames;
      }

      public ExpansionFirstCSharpRunner( params string[] templateDirectoryNames) : this(null,  templateDirectoryNames) { }

      public override string CreateString<TMeta, TTemplate>(TMeta metadata, TTemplate template)
      {
         if (typeof(TMetadata) != typeof(TMeta)) throw new InvalidOperationException();
         // TODO: Loosen the typing here as much as practical, interface?
         var xfTemplate = template as ExpansionFirstTemplate;
         if (xfTemplate == null) throw new InvalidOperationException("Template must be of type ExpansionFirstTemplate");
         var newRoots = xfTemplate.Run(metadata);
         var newRDomRoot = newRoots.First() as RDomRoot;
         var output = RDom.CSharp.GetFormattedSourceCode(newRDomRoot);
         return output;
      }

      protected override IEnumerable<ITemplate> GetTemplates()
      {
         var ret = new List<ITemplate>();
         foreach (var templateDirectory in templateDirectoryNames)
         {
            var partialFiles = Directory.GetFiles(templateDirectory, "*.partial.cs");
            var templateFiles = Directory.GetFiles(templateDirectory, "*.cs")
                                    .Except(partialFiles);
            foreach (var templateFile in templateFiles)
            {
               var xfTemplate = ExpansionFirstCSharp.LoadFromFile<TMetadata>(templateFile);
               ret.Add(xfTemplate);
            }
         }
         return ret;
      }
   }

}