using ExpansionFirst.Support;

namespace ExpansionFirstTemplateTests
{
   namespace NotifyPropertyChanged
   {
      using System.ComponentModel;

      //[[// There will almost always be just one class in the file. This also sets loop var name ]]
      #region [[ _xf_ForEach(LoopOver="Meta.Classes", VarName="Class") ]]
      namespace _xf_Class_dot_Namespace
      {
         //[[// Assumes that the source is a clean and disposable listing of properties that are candidates ]]
         //[[// for property changed and that there is a partial class with custom code and a base class ]]
         //[[// that includes a SetProperty method   ]]
         [_xf_.OutputWithoutPartial()]
         public sealed partial class _xf_Class_dot_Name : INotifyPropertyChanged
         {
            public event PropertyChangedEventHandler PropertyChanged;

            #region [[ _xf_ForEach(LoopOver="Class.Properties", VarName="Property") ]]
            #region Your own region to show it works

            // PropertyName:          _xf_Property_dot_Name
            // PropertyName as camel: _xf_Property_dot_Name_as_CamelCase
            private _xf_Property_dot_PropertyType_dot_Name _xf_Property_dot_Name_as_CamelCase;
            [_xf_.OutputXmlComments]
            [_xf_.OutputAttributes]
            public _xf_Property_dot_PropertyType_dot_Name _xf_Property_dot_Name
            {
               get { return _xf_Property_dot_Name_as_CamelCase; }
               set { SetProperty(ref _xf_Property_dot_Name_as_CamelCase, value); }
            }
            #endregion
            #endregion
         }
      }
      #endregion
   }
}
