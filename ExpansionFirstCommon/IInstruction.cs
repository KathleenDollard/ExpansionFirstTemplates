using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Collections.Generic;

namespace ExpansionFirst.Common
{
   public interface IInstruction
   {
      string Id { get; }
      //bool DoInstruction(
      //            IDom part,
      //            MetadataContextStack contextStack,
      //            List<IDom> retList,
      //            ref IDom lastPart,
      //            ref bool reRootTemplate);

      /* New Interface: Notes to self

         I contemplated separate interfaces for each type, but the recognition the instruction 
         is a match will be common to all, and I think it will be quite confusing to have 
         intialization for an instruction separate from how it is used. This does mean that
         many, most instructions will have a lot of empty items. 

         I need to add a reporter to the contextStack. 

         Instructions should probably be found via an IoC discovery mechanism. 

         There are no "Services" - any helper/support classes are on the stack 
         with whatever interface or implementation the custo instructions desire. 
         Thus they are called helpers or support. 

         I plan to supply:

            - Configuration (reads from a file (which might have a UI) for run constants)
            - String utilities
            - 

         Desire better solutions for:

            - standard mangline - not sure how manu other than fields and parameters/locals. 
                                - Regex doesn't have a ToUpper
                                - Is Camel, Pascal, Upper, Lower, Snake all that's needed?

      */


      /// <summary>
      /// Run for each part, RoslynDom item, of the template that is copied, before it is copied
      /// </summary>
      /// <param name="sharedPart">The part that is to be copied</param>
      /// <param name="contextStack">The stack of available data, variables and helpers</param>
      /// <param name="retList">The list of items this will be copied into</param>
      /// <param name="lastPart">The final part handled</param>
      /// <returns>True if the part has been handled and it should not be copied</returns>
      /// <remarks>
      /// (Rather obviously) instructions must provide dummy implementations of each method, but they
      /// will often have a meaningful implementation of only one. Empty implementations must
      /// return false.
      /// <para/>
      /// This instruction should not change the contents of part in a manner specific to this 
      /// iteration, since the shared copy of the part is the one passed. 
      /// </remarks>
      /// <example>
      /// In normal template processing ForEach uses BeforeCopy when it encounters a ForEach region. 
      /// It copies the content of the region using normal processing. When it is done, it returns tre
      /// because it itself should not be copied. It's contents have been copied into retList and the 
      /// lastPart is set to the #endregion so the next sibling causes processing to continue.
      /// </example>
      bool BeforeCopy(
                  IDom sharedPart,
                  MetadataContextStack contextStack,
                  List<IDom> retList,
                  ref IDom lastPart);

      /// <summary>
      /// Run for each newly created part, after it's chlidren have also been created
      /// </summary>
      /// <param name="newPart">The newly created part</param>
      /// <param name="contextStack">The stack of available data, variables and helpers</param>
      /// <param name="retList">The list of items this will be copied into</param>
      /// <remarks>
      /// (Rather obviously) instructions must provide dummy implementations of each method, but they
      /// will often have a meaningful implementation of only one.
      /// <para/>
      /// This instruction is designed for changes specific to this new instance.
      /// <para/>
      /// This runs AFTER the children have been copied. The logic for this is that if the children 
      /// need information from this new part, that information is better carried in the context stack
      /// and a before and after child copied seemed exceessive
      /// </remarks>
      /// <example>
      /// In normal template processing there is an attribute to remove the partial modifier on classes. 
      /// This modifier allows symbol, constant and method definition within the class for editing, but
      /// outside the template, so is almost required in a template. Not all outputshould be partials. 
      /// After other processing on the item, the instruction to remove the partial can be run.
      /// </example>
      void AfterCopy(
                  IDom newPart,
                  MetadataContextStack contextStack,
                  List<IDom> retList);

      /// <summary>
      /// Called before any processing. Can be used to alter the root, although it is most commonly
      /// used to alter the context stack. 
      /// </summary>
      /// <param name="sharedRoot">The root of the template</param>
      /// <param name="contextStack">The stack of available data, variables and helpers</param>
      /// <remarks>
      /// Things you push onto the context stack will overwrite any standard items already on the stack.
      /// <para/>
      /// A push is already done before each template run
      /// </remarks>
      void TemplateStart(IRoot sharedRoot,
                  MetadataContextStack contextStack);

      /// <summary>
      /// Called after all processing. Alters root for final output.
      /// </summary>
      /// <param name="newRoot">The root of the newly created item</param>
      /// <param name="contextStack">The stack of available data, variables and helpers</param>
      /// <remarks>
      /// If any expesnive resources are created during processing, that are not used for other
      /// templates, they should be finalized here. 
      /// <para/>
      /// A Pop is already done after template run
      /// </remarks>
      /// <example>
      /// Templates require outer information to create valid source code. At a minimum using  
      /// directives for the namespaces of the template itself are required. The TemplateStart
      /// instruction identifies the region of the template itself, allowing all the exterior 
      /// gunk you require. This instruction is not processed until Finalization, and then simply
      /// throws away anything that appears before or after the TemplateStart block. 
      /// </example>
      void TemplateEnd(IRoot newRoot,
                  MetadataContextStack contextStack);

      /// <summary>
      /// Initializes the run. Generally templates are applied sequentially to a series of metadata. 
      /// This method is run before any templates are processed.
      /// </summary>
      /// <param name="contextStack">The stack of available data, variables and helpers</param>
      /// <example>
      /// A configuration instruction could use this to supply variables to the template. I am planning
      /// to create this when I loop over files to provide file locations. It could also be used to create 
      /// additional services for templates. Services are provided via the contextStack, since it is needed
      /// for variables. Thus services for this project are called helpers or support. 
      /// </example>
      void RunStart(MetadataContextStack contextStack);

      /// <summary>
      /// The final method called. Intended as an opportunity to clean up any expensive resources and do 
      /// any other operations.
      /// </summary>
      /// <param name="contextStack">The stack of available data, variables and helpers</param>
      /// <example>
      /// Reporting is tidied up and output to a file. 
      /// </example>
      void RunEnd(MetadataContextStack contextStack);

   }
}
