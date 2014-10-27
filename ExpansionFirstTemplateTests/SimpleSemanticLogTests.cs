using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeFirstMetadataTest.SemanticLog;
using CodeFirst;
using RoslynDom;
using ExpansionFirstTemplates;
using System.Linq;
using RoslynDom.CSharp;
using RoslynDom.Common;

namespace ExpansionFirstTemplateTests
{
   [TestClass]
   public class SimpleSemanticLogTemplateTests
   {
      private string attributeIdentifier = "SemanticLog";
      private string metadataSource = @"
namespace ConsoleRunT4Example
{
    [SemanticLog()]
    public class Normal
    {
        public void Message(string Message) { }
        [Event(2)]
        public void AccessByPrimaryKey(int PrimaryKey) { }
    }

}";

      private string template = @"
//[[file:_xf_CopyForEach(LoopOver=""Meta__SemanticLogs"", LoopVarName=""_logClass_"")]]
using System;

//[[_xf_SetVariable(Fred: 42)]]
namespace _xf_class_namespaceName
{
    public partial class _xf_logClass_ClassName
    { }
";

      [TestMethod]
      public void Can_load_metadata()
      {
         var metadataLoader = new CodeFirstMetadataLoader<CodeFirstSemanticLogGroup>();
         CodeFirstSemanticLogGroup metadata = metadataLoader.LoadFromString(metadataSource, attributeIdentifier);
         Assert.IsNotNull(metadata);
      }

      [TestMethod]
      public void Can_load_tempate_as_tree()
      {
         var root = RDom.CSharp.Load(template);
         Assert.IsNotNull(root);
      }

      [TestMethod]
      public void Can_get_expansion_first_file_instructions()
      {
         var root = RDom.CSharp.Load(template);
         var instructions = root.GetMembers()
                           .OfType<IPublicAnnotation>()
                           .Where(x=>x.Target == "file")
                           .Where(x=>x.Name.StartsWith ("_xf_"));
         Assert.AreEqual(1, instructions.Count());
         var instruction = instructions.First();
         var loopOver = instruction.GetValue<string>("LoopOver");
         var loopVarName = instruction.GetValue<string>("LoopVarName");
         Assert.AreEqual("Meta__SemanticLogs", loopOver);
         Assert.AreEqual("_logClass_", loopVarName);
      }

      [TestMethod]
      public void Can_get_simple_output()
      {
         var xfTemplate = ExpansionFirstTemplate.Load(template);
         var metadataLoader = new CodeFirstMetadataLoader<CodeFirstSemanticLogGroup>();
         CodeFirstSemanticLogGroup metadata = metadataLoader.LoadFromString(metadataSource, attributeIdentifier);
         var newRoots = xfTemplate.Run(metadata);
         var newRDomRoot = newRoots.First() as RDomRoot;
         var outputSyntax = RDom.CSharp.GetSyntaxNode(newRDomRoot);
         var output = outputSyntax.ToString();
         Assert.Inconclusive();
      }
   }
}
