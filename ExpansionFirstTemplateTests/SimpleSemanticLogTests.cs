using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeFirstMetadataTest.SemanticLog;
using CodeFirst;
using RoslynDom;
using ExpansionFirstTemplates;
using System.Linq;
using RoslynDom.CSharp;
using RoslynDom.Common;
using CodeFirstMetadataTest.PropertyChanged;
using System.IO;

namespace ExpansionFirstTemplateTests
{
   [TestClass]
   public class SimpleSemanticLogTemplateTests
   {
      private string logAttributeIdentifier = "SemanticLog";
      private string logMetadataSource = @"
namespace ExpansionFirstExample
{
    [SemanticLog()]
    public class Normal
    {
        public void Message(string Message) { }
        [Event(2)]
        public void AccessByPrimaryKey(int PrimaryKey) { }
    }

}";

      /// <summary>
      /// 
      /// </summary>
      private string propChangedAttributeIdentifier = "NotifyPropertyChanged";
      private string propChangedMetadataSource = @"
namespace ExpansionFirstExample
{
    /// <summary>
    /// This is the class
    /// </summary>
    [NotifyPropertyChanged]
    public class Customer
    {
        /// <summary>
        /// This is the first name
        /// </summary>
        public string FirstName{get; set;}

        [TestAttribute]
        public string LastName{get; set;}

        public int  Id{get; set;}
        public DateTime  BirthDate{get; set;}
    }
}";

      private string template = @"
//[[file:_xf_CopyForEach(LoopOver=""Meta__SemanticLogs"", LoopVarName=""logClass_"")]]
using System;
//[[_xf_SetVariable(Fred=""FirstProperty"")]]
//[[_xf_SetVariable(George=42)]]
namespace _xf_class_namespaceName
{
   public partial class _xf_logClass_ClassName
   { 
      public string _xf_Fred {get; } = _xf_George;
   }
}";

      [TestMethod]
      public void Can_load_metadata()
      {
         var metadataLoader = new CodeFirstMetadataLoader<CodeFirstSemanticLogGroup>();
         CodeFirstSemanticLogGroup metadata = metadataLoader.LoadFromString(logMetadataSource, logAttributeIdentifier);
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
         Assert.AreEqual("logClass_", loopVarName);
      }

      [TestMethod]
      public void Can_get_simple_output()
      {
         var xfTemplate = ExpansionFirstCSharp.Load(template);
         var metadataLoader = new CodeFirstMetadataLoader<CodeFirstSemanticLogGroup>();
         CodeFirstSemanticLogGroup metadata = metadataLoader.LoadFromString(logMetadataSource, logAttributeIdentifier);
         var newRoots = xfTemplate.Run(metadata);
         var newRDomRoot = newRoots.First() as RDomRoot;
         var outputSyntax = RDom.CSharp.GetSyntaxNode(newRDomRoot);
         var output = outputSyntax.ToString();
         var expected = "using System;\r\nnamespace _xf_class_namespaceName\r\n{\r\n   public class _xf_logClass_ClassName\r\n   { \r\n      public string FirstProperty {get; }    }\r\n}";
         Assert.AreEqual(expected, output);
      }

      [TestMethod]
      public void Can_load_NotifyPropertyChanged_metadata()
      {
         var metadataLoader = new CodeFirstMetadataLoader<CodeFirstClassGroup>();
         CodeFirstClassGroup metadata = metadataLoader.LoadFromString(propChangedMetadataSource, propChangedAttributeIdentifier);
         Assert.IsNotNull(metadata);
      }


      [TestMethod]
      public void Can_get_NotifyPropertyChanged_output()
      {
         var csharpCode = File.ReadAllText(@"..\..\NotifyPropertyChanged.cs");
         var root = RDom.CSharp.Load(csharpCode);
         var verify = RDom.CSharp.GetSyntaxNode(root).ToFullString();
         //Assert.AreEqual(csharpCode, verify);

         TestCodeFirstClassGroup(@"..\..\NotifyPropertyChanged.cs");
      }

      [TestMethod]
      public void Can_get_NotifyPropertyChanged2_output()
      {
         TestCodeFirstClassGroup(@"..\..\NotifyPropertyChanged2.cs");
      }

      private void TestCodeFirstClassGroup(string templateFileName)
      {
         var xfTemplate = ExpansionFirstCSharp.LoadFromFile(templateFileName);
         var metadataLoader = new CodeFirstMetadataLoader<CodeFirstClassGroup>();
         CodeFirstClassGroup metadata = metadataLoader.LoadFromString(propChangedMetadataSource, propChangedAttributeIdentifier);
         var newRoots = xfTemplate.Run(metadata);
         var newRDomRoot = newRoots.First() as RDomRoot;
         var outputSyntax = RDom.CSharp.GetSyntaxNode(newRDomRoot);
         var output = outputSyntax.ToFullString();
         output = CleanOutput(output);
         Assert.Inconclusive();
      }


      // TODO: Fix this ugly aftermarket hack
      private string CleanOutput(string output)
      {
         output = output.SubstringAfter("_xf_TemplateStart()");
         output = output.SubstringAfter("\r\n");
         output = output.SubstringBeforeLast("#endregion");
         output = output            .SubstringBeforeLast("\r\n");
         return output;
      }

      [TestMethod]
      public void Can_get_super_simple_output()
      {
         var csharpCode = @"
namespace ExpansionFirstTemplateTests
{

      using System.ComponentModel;
      using System;
      #region _xf_MakeFileForEach(Over=asdf) 

      namespace _xf_Class_namespaceName
      {}
      #endregion
}
";
         var root = RDom.CSharp.Load(csharpCode);
         var verify = RDom.CSharp.GetSyntaxNode(root).ToFullString();
         Assert.AreEqual(csharpCode, verify);

         var xfTemplate = ExpansionFirstCSharp.Load(csharpCode);
         var metadataLoader = new CodeFirstMetadataLoader<CodeFirstSemanticLogGroup>();
         CodeFirstSemanticLogGroup metadata = metadataLoader.LoadFromString(logMetadataSource, logAttributeIdentifier);
         var newRoots = xfTemplate.Run(metadata);

         var newRDomRoot = newRoots.First() as RDomRoot;
         var outputSyntax = RDom.CSharp.GetSyntaxNode(newRDomRoot);
         var output = outputSyntax.ToFullString();
         var expected = "\r\nnamespace ExpansionFirstTemplateTests\r\n{\r\n\r\n      using System.ComponentModel;\r\n      using System;\r\n      #region _xf_MakeFileForEach(Over=asdf) \r\n\r\n      namespace _xf_Class_namespaceName\r\n      {}\r\n      #endregion\r\n}\r\n";

         Assert.AreEqual(expected, output);
      }

   }
}
