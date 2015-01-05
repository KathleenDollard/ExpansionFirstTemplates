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
using System;
using Microsoft.CodeAnalysis.MSBuild;

namespace DomainGenerationTests
{
   [TestClass]
   public class DomainGenerationTests
   {
      [TestClass]
      public class SimpleSemanticLogTemplateTests
      {
         private const string domainTemplateDirectory = @"..\..\..\DomainGenerationTemplates\";
         private const string superSimpleTemplateName = domainTemplateDirectory + @"SuperSimpleClassTemplate.cs";
         private const string propertyChangedTemplateName = domainTemplateDirectory + @"PropertyChangedTemplate.cs";


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
            var xfTemplate = ExpansionFirstCSharp.Load<CodeFirstClassGroup>(template);
            Assert.IsNotNull(xfTemplate);

            var metadataLoader = provider.GetMetadataLoader<CodeFirstClassGroup>();
            var metadata = metadataLoader.LoadFromString(propChangedMetadataSource, "");
            Assert.IsNotNull(metadata);

            var newRoots = xfTemplate.Run(metadata);
            var newRDomRoot = newRoots.First() as RDomRoot;
            var outputSyntax = RDom.CSharp.GetSyntaxNode(newRDomRoot);
            var output = outputSyntax.ToString();
            var expected = "namespace ExpansionFirstTemplatesTests.SuperSimpleClassTemplate\r\n{\r\n   namespace ExpansionFirstExample\r\n   {\r\n      public sealed class Customer\r\n      { }\r\n   }\r\n}\r\n";
            Assert.AreEqual(expected, output);
         }

         [TestMethod]
         public void DomainGeneration_excludes_code_outside_start_block()
         {
            var template = @"
namespace ExpansionFirstTemplatesTests.PropertyChangedTemplate
{
   #region [[ _xf_TemplateStart() ]]
   using System;
   using System.ComponentModel;

   #region [[ _xf_ForEach(LoopOver=""Meta.Classes"", VarName=""Class"") ]]
   namespace _xf_Class_dot_Namespace
   {
      public sealed partial class _xf_Class_dot_Name
      { }
   }
#endregion
#endregion
}";
            var provider = new ServiceProvider();
            var xfTemplate = ExpansionFirstCSharp.Load<CodeFirstClassGroup>(template);
            Assert.IsNotNull(xfTemplate);

            var metadataLoader = provider.GetMetadataLoader<CodeFirstClassGroup>();
            var metadata = metadataLoader.LoadFromString(propChangedMetadataSource, "");
            Assert.IsNotNull(metadata);

            var newRoots = xfTemplate.Run(metadata);
            var newRDomRoot = newRoots.First() as RDomRoot;
            var outputSyntax = RDom.CSharp.GetSyntaxNode(newRDomRoot);
            var output = outputSyntax.ToString();
            var expected = "using System;\r\n   using System.ComponentModel;\r\n\r\n   namespace ExpansionFirstExample\r\n   {\r\n      public sealed class Customer\r\n      { }\r\n   }\r\n";
            Assert.AreEqual(expected, output);
         }

         [TestMethod]
         public void DomainGeneration_can_handle_nested_ForEach()
         {
            var template = @"
namespace ExpansionFirstTemplatesTests.PropertyChangedTemplate
{
   #region [[ _xf_TemplateStart() ]]
   using System;
   using System.ComponentModel;

   #region [[ _xf_ForEach(LoopOver=""Meta.Classes"", VarName=""Class"") ]]
   namespace _xf_Class_dot_Namespace
   {
      public sealed partial class _xf_Class_dot_Name : INotifyPropertyChanged
      {
         public event PropertyChangedEventHandler PropertyChanged;
         #region [[ _xf_ForEach(LoopOver=""Class.Properties"", VarName=""Property"") ]]

         // Class name: _xf_Class_dot_Name / _xf_Class_dot_Name_as_CamelCase
         // Field/Property name: _xf_Property_dot_Name_as_CamelCase/_xf_Property_dot_Name 
         private _xf_Property_dot_PropertyType_dot_Name _xf_Property_dot_Name_as_CamelCase;
         public _xf_Property_dot_PropertyType_dot_Name _xf_Property_dot_Name
         {
            get { return _xf_Property_dot_Name_as_CamelCase; }
            set { SetProperty(ref _xf_Property_dot_Name_as_CamelCase, value); }
         }
         #endregion
      }
   }
   #endregion
   #endregion
}";
            var provider = new ServiceProvider();
            var xfTemplate = ExpansionFirstCSharp.Load<CodeFirstClassGroup>(template);
            Assert.IsNotNull(xfTemplate);

            var metadataLoader = provider.GetMetadataLoader<CodeFirstClassGroup>();
            var metadata = metadataLoader.LoadFromString(propChangedMetadataSource, "");
            Assert.IsNotNull(metadata);

            var newRoots = xfTemplate.Run(metadata);
            var newRDomRoot = newRoots.First() as RDomRoot;
            var outputSyntax = RDom.CSharp.GetSyntaxNode(newRDomRoot);
            var output = outputSyntax.ToString();
            var expected = "using System;\r\n   using System.ComponentModel;\r\n\r\n   namespace ExpansionFirstExample\r\n   {\r\n      public sealed class Customer : INotifyPropertyChanged\r\n      {\r\n         public event PropertyChangedEventHandler PropertyChanged;\r\n\r\n         // Class name: Customer / customer\r\n         // Field/Property name: firstName/FirstName \r\n         private String firstName;\r\n         public String FirstName\r\n         {\r\n            get { return firstName; }\r\n            set { SetProperty(ref firstName, value); }\r\n         }\r\n\r\n         // Class name: Customer / customer\r\n         // Field/Property name: lastName/LastName \r\n         private String lastName;\r\n         public String LastName\r\n         {\r\n            get { return lastName; }\r\n            set { SetProperty(ref lastName, value); }\r\n         }\r\n\r\n         // Class name: Customer / customer\r\n         // Field/Property name: id/Id \r\n         private Int32 id;\r\n         public Int32 Id\r\n         {\r\n            get { return id; }\r\n            set { SetProperty(ref id, value); }\r\n         }\r\n\r\n         // Class name: Customer / customer\r\n         // Field/Property name: birthDate/BirthDate \r\n         private DateTime birthDate;\r\n         public DateTime BirthDate\r\n         {\r\n            get { return birthDate; }\r\n            set { SetProperty(ref birthDate, value); }\r\n         }\r\n      }\r\n   }\r\n";
            Assert.AreEqual(expected, output);
         }

         [TestMethod]
         public void DomainGeneration_creates_correct_output_for_NotifyPropertyChanged()
         {
            var provider = new ServiceProvider();
            var template = File.ReadAllText(propertyChangedTemplateName);
            var xfTemplate = ExpansionFirstCSharp.Load<CodeFirstClassGroup>(template);
            Assert.IsNotNull(xfTemplate);

            var metadataLoader = provider.GetMetadataLoader<CodeFirstClassGroup>();
            var metadata = metadataLoader.LoadFromString(propChangedMetadataSource, "");
            var newRoots = xfTemplate.Run(metadata);
            var newRDomRoot = newRoots.First() as RDomRoot;
            var outputSyntax = RDom.CSharp.GetSyntaxNode(newRDomRoot);
            var output = outputSyntax.ToString();
            var expected = "using System;\r\n   using System.ComponentModel;\r\n\r\n   namespace ExpansionFirstExample\r\n   {\r\n      public class Customer : INotifyPropertyChanged\r\n      {\r\n         public event PropertyChangedEventHandler PropertyChanged;\r\n\r\n         private String firstName;\r\n         public String FirstName\r\n         {\r\n            get { return firstName; }\r\n            set { SetProperty(ref firstName, value); }\r\n         }\r\n\r\n         private String lastName;\r\n         public String LastName\r\n         {\r\n            get { return lastName; }\r\n            set { SetProperty(ref lastName, value); }\r\n         }\r\n\r\n         private Int32 id;\r\n         public Int32 Id\r\n         {\r\n            get { return id; }\r\n            set { SetProperty(ref id, value); }\r\n         }\r\n\r\n         private DateTime birthDate;\r\n         public DateTime BirthDate\r\n         {\r\n            get { return birthDate; }\r\n            set { SetProperty(ref birthDate, value); }\r\n         }\r\n      }\r\n   }\r\n";
            Assert.AreEqual(expected, output);
         }

         [TestMethod]
         public void Should_create_domain_file_from_project()
         {
            var runner = new ExpansionFirstCSharpRunner<CodeFirstClassGroup>(domainTemplateDirectory);
            var startDirectory = Path.Combine(FileSupport.ProjectPath(AppDomain.CurrentDomain.BaseDirectory), 
                                 "..\\DomainGenerationMetadata");
            startDirectory = Path.GetFullPath(startDirectory);
            var ws = MSBuildWorkspace.Create();
            var projectPath = FileSupport.GetNearestCSharpProject(startDirectory);
            // For testing: wait for the result
            var project = ws.OpenProjectAsync(projectPath).Result;
            var dict = runner.CreateOutputStringsFromProject(project, "..\\Output");
            AssertCreation(dict, Tuple.Create("customer.g.cs", expected1), Tuple.Create("customerSuperSimple.g.cs", expected2));
         }

         //[TestMethod]
         //public void Should_create_domain_file_from_from_path()
         //{
         //   var runner = new T4TemplateRunner();
         //   var relativePath = "..\\DomainGenerationMetadata";
         //   var dict = runner.CreateOutputStringsFromProject(relativePath, "..\\Output");
         //   AssertCreation(dict);
         //}

         private void AssertCreation(IDictionary<string, string> dict, params Tuple<string, string>[] resultPairs)
         {
            Assert.AreEqual(resultPairs.Count(), dict.Count());

            foreach (var pair in resultPairs)
            {
               var expectedFileName = pair.Item1;
               var expectedData = pair.Item2;
               var actualPair = dict
                              .Where(x => x.Key.EndsWith(expectedFileName,StringComparison.OrdinalIgnoreCase))
                              .FirstOrDefault();
               Assert.IsNotNull(actualPair);
               var actualResult = actualPair.Value;
               actualResult = StringUtilities.RemoveFileHeaderComments(actualResult);
               Assert.AreEqual(expectedData, actualResult);
            }
         }

         private string expected1 = @"using System;
using System.ComponentModel;

namespace CodeFirstTest
{
    public class Customer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private String firstName;
        public String FirstName
        {
            get { return firstName; }
            set
            {
                SetProperty(ref firstName, value);
            }
        }

        private String lastName;
        public String LastName
        {
            get { return lastName; }
            set
            {
                SetProperty(ref lastName, value);
            }
        }

        private Int32 id;
        public Int32 Id
        {
            get { return id; }
            set
            {
                SetProperty(ref id, value);
            }
        }

        private DateTime birthDate;
        public DateTime BirthDate
        {
            get { return birthDate; }
            set
            {
                SetProperty(ref birthDate, value);
            }
        }
    }
}";

         private string expected2 = @"namespace ExpansionFirstTemplatesTests.SuperSimpleClassTemplate
{
    namespace CodeFirstTest
    {
        public sealed class Customer
        { }
    }
}";

      }
   }
}
