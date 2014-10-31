using System;
using ExpansionFirst.Common;
using RoslynDom.Common;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace ExpansionFirstTemplates
{
   public class ReplacementAlteration : IAlteration
   {
      public string Id
      { get { return "Replacement"; } }

      private Regex matchRegEx = new Regex(@"_xf_(?<part>[_\w]+)");
      //private Regex matchRegEx = new Regex(@"_xf_(?<part>[_\w]+?)(?<format>_as_\w+)*");

      public void DoAlteration(IDom item, MetadataContextStack contextStack)
      {
         ReplaceInName(item, contextStack);
         // TODO: Explore whether to cache this, the same items will be pounded
         IEnumerable<PropertyInfo> props = item.GetType().GetProperties();
         ReplaceInPropertiesNames(item, contextStack, props);
         ReplaceInText(item, contextStack, props);
         ReplaceInExpressions(item, contextStack, props);
      }

      private void ReplaceInName(IDom item, MetadataContextStack contextStack)
      {
         var partHasName = item as IHasName;
         if (partHasName != null)
         {
            var str = ReplacementString(partHasName.Name, contextStack);
            // don't reset if not needed, because that will enventually invalidate the model
            if (str != partHasName.Name) partHasName.Name = str;
         }
      }

      private void ReplaceInPropertiesNames(IDom item, MetadataContextStack contextStack, IEnumerable<PropertyInfo> props)
      {
         var filteredProps = props.Where(x => typeof(IHasName).IsAssignableFrom(x.PropertyType));
         foreach (var prop in filteredProps)
         {
            var propAsHasName = prop.GetValue(item) as IHasName;
            if (propAsHasName != null)  // underlying value can be null
            {
               var str = ReplacementString(propAsHasName.Name, contextStack);
               if (str != propAsHasName.Name) propAsHasName.Name = str;
            }
         }
         return;
      }

      private void ReplaceInText(IDom item, MetadataContextStack contextStack, IEnumerable<PropertyInfo> props)
      {
         var textProp = props.Where(x => x.Name == "Text").FirstOrDefault();
         if (textProp == null) return;
         var startString = textProp.GetValue(item) as string;
         var str = ReplacementInsideString(startString, contextStack);
         if (str != startString) textProp.SetValue(item, str);
      }

       private void ReplaceInExpressions(IDom item, MetadataContextStack contextStack, IEnumerable<PropertyInfo> props)
      {
         var filteredProps = props.Where(x => typeof(IExpression).IsAssignableFrom(x.PropertyType));
         foreach (var prop in filteredProps)
         {
            var expr = prop.GetValue(item) as IExpression;
            if (expr == null) return; // underlying value might be null
            var str = ReplacementInsideString(expr.Expression, contextStack);
            if (str != expr.Expression) expr.Expression = str;
         }
         return;
      }

      private string ReplacementInsideString(string input, MetadataContextStack contextStack)
      {
         var ret = input;
         var matches = matchRegEx.Matches(input);
         for (int i = 0; i < matches.Count; i++)
         {
            var match = matches[i];
            var str = match.Value;
            var newStr = GetNewString(str, contextStack, match);
            if (newStr != str) ret = ret.Replace(str, newStr);
         }
         return ret;
      }


      private string ReplacementString(string input, MetadataContextStack contextStack)
      {
         var match = matchRegEx.Match(input);
         return GetNewString(input, contextStack, match);
      }

      private static string GetNewString(string input, MetadataContextStack contextStack, Match match)
      {
         if (!match.Success) return input;
         var format = "";
         var replaceWith = match.Groups[1].Value;
         List<string> replaceParts = replaceWith
                           .Split(new string[] { "_dot_" }, StringSplitOptions.None)
                           .ToList();
         if (replaceParts.Last().Contains("_as_"))
         {
            var lastPart = replaceParts.Last();
            format = lastPart.SubstringAfter("_as_");
            lastPart = lastPart.SubstringBefore("_as_");
            replaceParts.Remove(replaceParts.Last());
            replaceParts.Add(lastPart);
         }
         var lookup = contextStack.GetValue(replaceParts.First());
         if (lookup == null) return input;
         replaceParts = replaceParts.Skip(1).ToList(); // get tail
         foreach (var part in replaceParts)
         {
            var lookupProperty = lookup.GetType().GetProperty(part);
            if (lookupProperty == null) return input;
            lookup = lookupProperty.GetValue(lookup);
            if (lookup == null) return input;
         }
         return GetWithFormat(lookup.ToString(), format);
      }

      /// <summary>
      /// Provides standard formatting to strings
      /// </summary>
      /// <param name="v"></param>
      /// <returns></returns>
      /// <remarks>
      /// In the long term, this needs to be an extension point, but it deserves
      /// some thought because I don't know if a standard interface makes sense
      /// when regex and normal formatting strings seem most likely. Also, a string
      /// is passed here, where an object might be valuable for certian formatting.
      /// For now, the formatting must be explicitly done right here. 
      /// </remarks>
      private static string GetWithFormat(string input, string format)
      {
         switch (format)
         {
            case Constants.FormatCamelCase:
               return char.ToLower(input[0]) + input.Substring(1);
            case Constants.FormatUpperCase:
               return input.ToUpper();
            case Constants.FormatLowerCase:
               return input.ToLower();
            case Constants.FormatPascalCase:
               return char.ToUpper(input[0]) + input.Substring(1);
            default:
               return input;
         }
      }
   }
}
