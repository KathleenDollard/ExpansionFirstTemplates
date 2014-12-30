using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom.Common;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using CodeFirstMetadataTest.PropertyChanged;
using RoslynDom;
using RoslynDom.CSharp;
using CodeFirst.Provider;
using ExpansionFirstTemplates;

namespace DomainGenerationTests
{
   [TestClass]
   public class DomainGenerationTests
   {
      [TestClass]
      public class SimpleSemanticLogTemplateTests
      {
         private string superSimpleTemplateName = @"..\..\..\DomainGenerationTemplates\SuperSimpleClassTemplate.cs";


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

 
         [TestMethod]
         public void DomainGeneration_can_load_metadata()
         {
            var provider = new ServiceProvider();
            var metadataLoader = provider.GetMetadataLoader<CodeFirstClassGroup>();
            var metadata = metadataLoader.LoadFromString(propChangedMetadataSource, "");
            Assert.IsNotNull(metadata);
         }

         [TestMethod]
         public void DomainGeneration_can_load_template_as_tree()
         {
            var template = File.ReadAllText(superSimpleTemplateName);
            var root = RDom.CSharp.Load(template);
            Assert.IsNotNull(root);
         }

         [TestMethod]
         public void DomainGeneration_can_get_simple_output()
         {
            var provider = new ServiceProvider();
            var template = File.ReadAllText(superSimpleTemplateName);
            var xfTemplate = ExpansionFirstCSharp.Load(template);
            var metadataLoader = provider.GetMetadataLoader<CodeFirstClassGroup>();
            var metadata = metadataLoader.LoadFromString(propChangedMetadataSource, "");
            var newRoots = xfTemplate.Run(metadata);
            var newRDomRoot = newRoots.First() as RDomRoot;
            var outputSyntax = RDom.CSharp.GetSyntaxNode(newRDomRoot);
            var output = outputSyntax.ToString();
            var expected = "namespace ExpansionFirstTemplatesTests\r\n{\r\n   namespace ExpansionFirstExample\r\n   {\r\n      public sealed class Customer\r\n      { }\r\n   }\r\n}\r\n";
            Assert.AreEqual(expected, output);
         }

         //[TestMethod]
         //public void Can_load_NotifyPropertyChanged_metadata()
         //{
         //   var metadataLoader = new CodeFirstMetadataLoader<CodeFirstClassGroup>();
         //   CodeFirstClassGroup metadata = metadataLoader.LoadFromString(propChangedMetadataSource, propChangedAttributeIdentifier);
         //   Assert.IsNotNull(metadata);
         //}


         //[TestMethod]
         //public void Can_get_NotifyPropertyChanged_output()
         //{
         //   var csharpCode = File.ReadAllText(@"..\..\NotifyPropertyChanged.cs");
         //   var root = RDom.CSharp.Load(csharpCode);
         //   var verify = RDom.CSharp.GetSyntaxNode(root).ToFullString();
         //   //Assert.AreEqual(csharpCode, verify);

         //   TestCodeFirstClassGroup(@"..\..\NotifyPropertyChanged.cs");
         //}

         //[TestMethod]
         //public void Can_get_NotifyPropertyChanged2_output()
         //{
         //   TestCodeFirstClassGroup(@"..\..\NotifyPropertyChanged2.cs");
         //}

         //private void TestCodeFirstClassGroup(string templateFileName)
         //{
         //   var xfTemplate = ExpansionFirstCSharp.LoadFromFile(templateFileName);
         //   var metadataLoader = new CodeFirstMetadataLoader<CodeFirstClassGroup>();
         //   CodeFirstClassGroup metadata = metadataLoader.LoadFromString(propChangedMetadataSource, propChangedAttributeIdentifier);
         //   var newRoots = xfTemplate.Run(metadata);
         //   var newRDomRoot = newRoots.First() as RDomRoot;
         //   var outputSyntax = RDom.CSharp.GetSyntaxNode(newRDomRoot);
         //   var output = outputSyntax.ToFullString();
         //   output = CleanOutput(output);
         //   Assert.Inconclusive();
         //}

         // TODO: Fix this ugly aftermarket hack
         private string CleanOutput(string output)
         {
            output = output.SubstringAfter("_xf_TemplateStart()");
            output = output.SubstringAfter("\r\n");
            output = output.SubstringBeforeLast("#endregion");
            output = output.SubstringBeforeLast("\r\n");
            return output;
         }

//         [TestMethod]
//         public void Can_get_super_simple_output()
//         {
//            var csharpCode = @"
//namespace ExpansionFirstTemplateTests
//{

//      using System.ComponentModel;
//      using System;
//      #region _xf_MakeFileForEach(Over=asdf) 

//      namespace _xf_Class_namespaceName
//      {}
//      #endregion
//}
//";
//            var root = RDom.CSharp.Load(csharpCode);
//            var verify = RDom.CSharp.GetSyntaxNode(root).ToFullString();
//            Assert.AreEqual(csharpCode, verify);

//            var xfTemplate = ExpansionFirstCSharp.Load(csharpCode);
//            var metadataLoader = new CodeFirstMetadataLoader<CodeFirstSemanticLogGroup>();
//            CodeFirstSemanticLogGroup metadata = metadataLoader.LoadFromString(logMetadataSource, logAttributeIdentifier);
//            var newRoots = xfTemplate.Run(metadata);

//            var newRDomRoot = newRoots.First() as RDomRoot;
//            var outputSyntax = RDom.CSharp.GetSyntaxNode(newRDomRoot);
//            var output = outputSyntax.ToFullString();
//            var expected = "\r\nnamespace ExpansionFirstTemplateTests\r\n{\r\n\r\n      using System.ComponentModel;\r\n      using System;\r\n      #region _xf_MakeFileForEach(Over=asdf) \r\n\r\n      namespace _xf_Class_namespaceName\r\n      {}\r\n      #endregion\r\n}\r\n";

//            Assert.AreEqual(expected, output);
//         }

         //[TestMethod]
         //public void Should_create_domain_file_from_project()
         //{
         //   var runner = new T4TemplateRunner();
         //   var startDirectory = Path.Combine(FileSupport.ProjectPath(AppDomain.CurrentDomain.BaseDirectory), "..\\DomainGenerationMetadata");
         //   startDirectory = Path.GetFullPath(startDirectory);
         //   var ws = MSBuildWorkspace.Create();
         //   var projectPath = FileSupport.GetNearestCSharpProject(startDirectory);
         //   // For now: wait for the result
         //   var project = ws.OpenProjectAsync(projectPath).Result;
         //   var dict = runner.CreateOutputStringsFromProject(project, "..\\Output");
         //   AssertCreation(dict);
         //}

         //[TestMethod]
         //public void Should_create_domain_file_from_from_path()
         //{
         //   var runner = new T4TemplateRunner();
         //   var relativePath = "..\\DomainGenerationMetadata";
         //   var dict = runner.CreateOutputStringsFromProject(relativePath, "..\\Output");
         //   AssertCreation(dict);
         //}

         [TestMethod]
         private void AssertCreation(IDictionary<string, string> dict)
         {
            Assert.AreEqual(2, dict.Count());
            var actual = dict
                           .Where(x => x.Key.EndsWith("Customer2.g.cs"))
                           .First()
                           .Value;
            actual = StringUtilities.RemoveFileHeaderComments(actual);
            Assert.AreEqual(expected1, actual);
            actual = dict
                           .Where(x => x.Key.EndsWith("Customer.g.cs"))
                           .First()
                           .Value;
            actual = StringUtilities.RemoveFileHeaderComments(actual);
            Assert.AreEqual(expected2, actual);
         }

         private string expected1 = @"using System;
using System.ComponentModel;

namespace CodeFirstTest
{
   public class Customer : INotifyPropertyChanged
   {
      public event PropertyChangedEventHandler PropertyChanged;

      private String firstName
      public public String FirstName
      {
         get { return firstName; }
         set { SetProperty(ref firstName, value); }
      }

      private String lastName
      public public String LastName
      {
         get { return lastName; }
         set { SetProperty(ref lastName, value); }
      }

      private Int32 id
      public public Int32 Id
      {
         get { return id; }
         set { SetProperty(ref id, value); }
      }

      private DateTime birthDate
      public public DateTime BirthDate
      {
         get { return birthDate; }
         set { SetProperty(ref birthDate, value); }
      }

   }
}
";

         private string expected2 = @"using System;
using System.ComponentModel;

namespace CodeFirstTest
{
   
   public class Customer : INotifyPropertyChanged
   
   {
      private String _firstName;
      private String _lastName;
      private Int32 _id;
      private DateTime _birthDate;
      
      public event PropertyChangedEventHandler PropertyChanged;
      
      protected virtual void OnPropertyChanged(string name)
      {
          if (this.PropertyChanged != null)
          {
              this.PropertyChanged(this, new PropertyChangedEventArgs(name));
          }
      }
      
      public String FirstName
      {
         get { return _firstName; }
         set
         {
            if (_firstName != value)
            {
               _firstName = value;
               this.OnPropertyChanged(""FirstName"");
            }
         }
      }
      public String LastName
      {
         get { return _lastName; }
         set
         {
            if (_lastName != value)
            {
               _lastName = value;
               this.OnPropertyChanged(""LastName"");
            }
         }
      }
      public Int32 Id
      {
         get { return _id; }
         set
         {
            if (_id != value)
            {
               _id = value;
               this.OnPropertyChanged(""Id"");
            }
         }
      }
      public DateTime BirthDate
      {
         get { return _birthDate; }
         set
         {
            if (_birthDate != value)
            {
               _birthDate = value;
               this.OnPropertyChanged(""BirthDate"");
            }
         }
      }
   }
}
";

      }
   }
}
