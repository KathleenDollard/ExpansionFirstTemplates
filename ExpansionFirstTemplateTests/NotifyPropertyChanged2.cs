namespace ExpansionFirstTemplateTests
{
   namespace NotifyPropertyChanged2
   {
      using ExpansionFirst.Common;
      #region [[ _xf_TemplateStart() ]]
      using System.ComponentModel;

      //[[// There will almost always be just one class in the file. This also sets loop var name ]]
      #region [[ _xf_ForEach(LoopOver="Meta.Classes", VarName="Class") ]]
      namespace _xf_Class_dot_Namespace
      {
         [_xf_.StructuredDocs(_xf_Class)]
         public sealed partial class _xf_Class_dot_Name : INotifyPropertyChanged
         {
            // Instruction exercise
            private int _xf_SetVariable_Fred = 42;
            //[[ _xf_SetVariable(George=44) ]]
            // Fred values _xf_Fred, _xf_George

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
}
