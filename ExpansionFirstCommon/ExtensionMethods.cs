using RoslynDom.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpansionFirst.Common
{
   public static class ExtensionMethods
   {
      public static IDom NextSibling(this IDom item)
      {
         if (item == null) return null;
         var container = item.Ancestors.OfType<IContainer>().First();
         var members = container.GetMembers();
         return members.FollowingSiblings(item).FirstOrDefault();
      }
   }
}
