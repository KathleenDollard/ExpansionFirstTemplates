using ExpansionFirst.Support;

namespace ExpansionFirstTemplateTests
{
   namespace NotifyPropertyChanged
   {
      #region  _xf_MakeFileForEach(Over="Meta.Classes", VarName="class_") 
      using System.ComponentModel;

      //[[// There will almost always be just one class in the file. This also sets loop var name ]]
      #region [[ _xf_ForEach(LoopOver="Meta.Classes", VarName="Class") ]]
      namespace _xf_Class_namespaceName
      {
         //[[// Assumes that the source is a clean and disposable listing of properties that are candidates ]]
         //[[// for property changed and that there is a partial class with custom code and a base class ]]
         //[[// that includes a SetProperty method   ]]
         [_xf_.OutputAsPartial()]
         public sealed partial class _xf_Class_dot_Name : INotifyPropertyChanged
         {
            public event PropertyChangedEventHandler PropertyChanged;

            #region [[ _xf_ForEach(LoopOver="_xf_Class_dot_Properties", LoopVar="Property") ]]
            #region Your properties - just included so you could see normal regions still work

            private _xf_Property_dot_Type _xf_Camel_Property_dot_Name;
            [_xf_.OutputXmlComments]
            [_xf_.OutputAttributes]
            public _xf_Property_dot_Type _xf_Property_dot_Name
            {
               get { return _xf_Camel_Property_dot_Name; }
               set { SetProperty(ref _xf_Camel_Property_dot_Name, value); }
            }
            #endregion
            #endregion
         }
      }

      #endregion

      #endregion
   }
}