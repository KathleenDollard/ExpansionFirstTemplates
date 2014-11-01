#define xf_special

using CodeFirstMetadataTest.SemanticLog;
using ExpansionFirst.Common;

namespace ExpansionFirstTemplateTests2._xf_class_namespaceName
{


   public partial class _xf_logClass_dot_ClassName
   {
      public const string _xf_logClass_UniqueName = null; //[[ Equals _logClass.UniqueName ]]
      public const int _xf_Event_EventId = 0;             //[[ Equals _event.EvnetId ]]
      public static T _xf_ArgumentsFrom<T>(params object[] parms) { return default(T); }
   }
}

namespace ExpansionFirstTemplateTests2
{
   #region  _xf_MakeOne(Over="Meta", VarName="_logClass_") 
   using System;
   using System.Diagnostics.Tracing;

   namespace _xf_class_namespaceName
   {
      #region _xf_ForEach(LoopOver="Meta.LogClass", LoopVarName="_logClass")
      [EventSource(Name = _xf_logClass_UniqueName)]
      public sealed partial class _xf_logClass_dot_ClassName : EventSource
      {
         #region Standard class stuff
         // Private constructor blocks direct instantiation of class
         private _xf_logClass_dot_ClassName() { }

         // Readonly access to cached, lazily created singleton instance
         private static readonly Lazy<_xf_logClass_dot_ClassName> _lazyLog =
                 new Lazy<_xf_logClass_dot_ClassName>(() => new _xf_logClass_dot_ClassName());
         public static _xf_logClass_dot_ClassName Log
         {
            get { return _lazyLog.Value; }
         }

         // Readonly access to  private cached, lazily created singleton inner class instance
         private static readonly Lazy<_xf_logClass_dot_ClassName> _lazyInnerlog =
              new Lazy<_xf_logClass_dot_ClassName>(() => new _xf_logClass_dot_ClassName());
         private static _xf_logClass_dot_ClassName innerLog
         {
            get { return _lazyInnerlog.Value; }
         }
         #endregion

         #region _xf_ForEach(LoopOver="_logClass_dot_Events")
         private CodeFirstLogEvent _xf_Event; //[[ LoopVar ]]
         #region Your trace event methods

         [Event(_xf.Event_EventId)]
         [_xf_.AddStructuredDocs]
         [_xf_.AddAttributes]
         void _xf_Event_dot_Name()
         {
            if (IsEnabled()) WriteEvent(_xf_Event.EventId, _xf.ArgumentsFrom<object>(_xf_Event));
         }
         #endregion
         #endregion
      }

      #endregion
   }
   #endregion
}
