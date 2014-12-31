namespace ExpansionFirstTemplatesTests.DiagnosticAnalyzer
{
   using _xf_Analyzer_dot_PropertyType_dot_Name = Microsoft.CodeAnalysis.SyntaxNode;

   #region [[ _xf_TemplateStart() ]]
   using System;
   using System.Collections.Immutable;
   using System.Linq;
   using System.Threading;
   using Microsoft.CodeAnalysis;
   using Microsoft.CodeAnalysis.Diagnostics;
   using Microsoft.CodeAnalysis.CSharp;
   using Microsoft.CodeAnalysis.CSharp.Syntax;


   #region [[ _xf_ForEach(LoopOver="Meta.Diagnostics", VarName="Diagnostic") ]]
   namespace _xf_Diagnostic_dot_Namespace
   {
      public partial class _xf_Diagnostic_dot_Name : DiagnosticAnalyzer
      {
         public const string DiagnosticId = _xf_Diagnostic_dot_Id;
         internal const string Description = _xf_Diagnostic_dot_Description;
         internal const string MessageFormat = _xf_Diagnostic_dot_MessageFormat;
         internal const string Category = _xf_Diagnostic_dot_Category;

         internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Error, true);

         public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

         public override void Initialize(AnalysisContext context)
         {
            context.RegisterSyntaxNodeAction(AnalyzeNodes, _xf_Diagnostic_dot_Analyzers_dot_SyntaxKind_as_CommaJoin);
         }

         private void AnalyzeNodes(SyntaxNodeAnalysisContext context)
         {
            #region [[ _xf_ForEach(LoopOver="Diagnostic.Analyzers", VarName="Analyzer") ]]


            var _xf_Analyzer_dot_VariableName = context.Node as _xf_Analyzer_dot_PropertyType_dot_Name;

            if (_xf_Analyzer_dot_VariableName != null
                  && (_xf_Analyzer_dot_ConditionString_as_Trimmed))
            {
               Location loc = _xf_Analyzer_dot_GetLocationString_as_Trimmed;
               Diagnostic diagnostic = Diagnostic.Create(Rule, loc, _xf_Analyzer_dot_MessageArg_as_Trimmed);
               context.ReportDiagnostic(diagnostic);
            }
            #endregion
         }
      }
   }
   #endregion
   #endregion
}