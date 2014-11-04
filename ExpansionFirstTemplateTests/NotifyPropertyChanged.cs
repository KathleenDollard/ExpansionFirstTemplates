namespace ExpansionFirstTemplateTests
{
   namespace NotifyPropertyChanged
   {
      #region [[ _xf_TemplateStart() ]]
      using System.ComponentModel;

      //[[// There will almost always be just one class in the file. This also sets loop var name ]]
      #region [[ _xf_ForEach(LoopOver="Meta.Classes", VarName="Class") ]]
      namespace _xf_Class_dot_Namespace
      {
         //[[// Assumes that the source is a clean and disposable listing of properties that are candidates ]]
         public sealed partial class _xf_Class_dot_Name : INotifyPropertyChanged
         {
            public event PropertyChangedEventHandler PropertyChanged;
            #region [[ _xf_ForEach(LoopOver="Class.Properties", VarName="Property") ]]

            // Property name/field name: _xf_Property_dot_Name / _xf_Property_dot_Name_as_CamelCase
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
}
