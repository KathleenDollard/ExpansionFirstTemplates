//[[ _xf_FilePathHint("{ExecutionPath}\..\..\..\DomainOutput\{MetadataFileName}.g.cs") ]]
namespace ExpansionFirstTemplatesTests.PropertyChangedTemplate
{
   #region [[ _xf_TemplateStart() ]]
   using System;
   using System.ComponentModel;

   #region [[ _xf_ForEach(LoopOver="Meta.Classes", VarName="Class") ]]
   namespace _xf_Class_dot_Namespace
   {
      public partial class _xf_Class_dot_Name : INotifyPropertyChanged
      {
         public event PropertyChangedEventHandler PropertyChanged;
         #region [[ _xf_ForEach(LoopOver="Class.Properties", VarName="Property") ]]

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
}