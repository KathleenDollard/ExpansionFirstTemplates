#define xf_special

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFirstMetadataTest.SemanticLog;

public class _xf
{
   public const string logClass_UniqueName = null; //[[ Equals _logClass.UniqueName ]]
   public const int Event_EventId = 0; //[[ Equals _event.EvnetId ]]
   public static T ArgumentsFrom<T>(params object[] parms) { return default(T); }
}

namespace ExpansionFirstTemplateTests
{

   #region  _xf_MakeOne(Over="Meta", VarName="_logClass_") 
   using System;
   using System.Diagnostics.Tracing;

   namespace _xf_class_namespaceName
   {
      #region _xf_ForEach(LoopOver="Meta.LogClass", LoopVarName="_logClass")
      [EventSource(Name = _xf.logClass_UniqueName)]
      public sealed partial class _logClass_dot_ClassName : EventSource
      {
         #region Standard class stuff
         // Private constructor blocks direct instantiation of class
         private _logClass_dot_ClassName() { }

         // Readonly access to cached, lazily created singleton instance
         private static readonly Lazy<_logClass_dot_ClassName> _lazyLog =
                 new Lazy<_logClass_dot_ClassName>(() => new _logClass_dot_ClassName());
         public static _logClass_dot_ClassName Log
         {
            get { return _lazyLog.Value; }
         }

         // Readonly access to  private cached, lazily created singleton inner class instance
         private static readonly Lazy<_logClass_dot_ClassName> _lazyInnerlog =
              new Lazy<_logClass_dot_ClassName>(() => new _logClass_dot_ClassName());
         private static _logClass_dot_ClassName innerLog
         {
            get { return _lazyInnerlog.Value; }
         }
         #endregion

         #region _xf_ForEach(LoopOver="_logClass_dot_Events")
         private CodeFirstLogEvent _xf_Event; //[[ LoopVar ]]
         #region Your trace event methods
         #region _xf_If(condition)
         //[[ OutputXmlComments(_xf_Event); ]]
         #endregion
         //[[ OutputAttributes(_xf_Event); ]]

         [Event(_xf.Event_EventId)]
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
