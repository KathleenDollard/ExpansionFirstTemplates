using RoslynDom.Common;

namespace ExpansionFirst.Common
{
   public interface IAlteration
   {
      string Id { get; }
      void DoAlteration(IDom item, MetadataContextStack contextStack);
   }
}
