using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SimplyAssociate.Utilities
{
    internal static class CodeElementExtension
    {
        internal static string GetAttributeValue(this CodeElement codeElement, string attributeName)
        {
            string value = string.Empty;
            try
            {
                CodeElements attributes = ((EnvDTE80.CodeFunction2)codeElement).Attributes;
                foreach (CodeElement currAttribute in attributes)
                {
                    if (currAttribute.Name == attributeName)
                    {
                        value = ((EnvDTE80.CodeAttribute2)currAttribute).Value;
                        break;
                    }
                }
            }
            catch
            {
                value = string.Empty;
            }
            return value;
        }

        internal static bool IsAttributeExist(this CodeElement codeElement, string attributeName)
        {
            if (codeElement.Kind != vsCMElement.vsCMElementFunction && codeElement.Kind != vsCMElement.vsCMElementClass)
                throw new InvalidCastException(string.Format(ErrorMessages.CODEELEMENT_UNKNOWNKIND, codeElement.Kind.ToString()));
            CodeElements attributes = ((EnvDTE80.CodeFunction2)codeElement).Attributes;
            return attributes.Cast<CodeElement>().Any(e => e.Name == attributeName);
        }

        internal static Dictionary<string, string> GetAttributes(this CodeElement codeElement)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            CodeElements elemAttributes = null;
            if (codeElement.Kind == vsCMElement.vsCMElementFunction)
            {
                elemAttributes = ((EnvDTE80.CodeFunction2)codeElement).Attributes;
            }
            else if (codeElement.Kind == vsCMElement.vsCMElementClass)
            {
                elemAttributes = ((EnvDTE80.CodeFunction2)codeElement).Attributes;
            }

            if (elemAttributes == null)
                throw new InvalidCastException(string.Format(ErrorMessages.CODEELEMENT_UNKNOWNKIND, codeElement.Kind.ToString()));

            foreach (CodeElement attribute in elemAttributes)
            {
                string name = attribute.Name;
                string value = string.Empty;
                try
                {
                    value = ((EnvDTE80.CodeAttribute2)attribute).Value;
                }
                catch { }
                attributes.Add(name, value);
            }
            return attributes;
        }
    }
}
