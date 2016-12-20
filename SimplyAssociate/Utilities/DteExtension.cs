using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SimplyAssociate.Utilities
{
    internal static class DteExtension
    {
        internal static Document GetActiveDocument(this EnvDTE.DTE _dte)
        {
            return _dte.ActiveDocument;
        }

        internal static TextDocument GetActiveTextDocument(this EnvDTE.DTE _dte)
        {
            return _dte.ActiveDocument.Object() as TextDocument;
        }

        internal static Project GetActiveProject(this EnvDTE.DTE _dte)
        {
            return _dte.GetActiveTextDocument().Parent.ProjectItem.ContainingProject;
        }

        internal static Configuration GetConfigurationForActiveProject(this EnvDTE.DTE _dte)
        {
            return _dte.GetActiveProject().ConfigurationManager.ActiveConfiguration;
        }

        internal static string GetOutputFileName(this Project _project)
        {
            return _project.Properties.Item("OutputFileName").Value.ToString();
        }

        internal static string GetOutputDirectoryPath(this Project _project)
        {
            return _project.Properties.Item("FullPath").Value.ToString();
        }

        internal static TextSelection GetActiveTextSelection(this EnvDTE.DTE _dte)
        {
            return (TextSelection)_dte.GetActiveDocument().Selection;
        }

        internal static CodeElement GetFunctionAtCursor(this TextDocument _textDocument, out TextPoint textPoint)
        {
            CodeElement functionAtCursor = null;
            try
            {
                TextSelection sel = (TextSelection)_textDocument.Selection;
                textPoint = (TextPoint)sel.ActivePoint;
                functionAtCursor = textPoint.get_CodeElement(vsCMElement.vsCMElementFunction);
            }
            catch
            {
                textPoint = null;
            }
            return functionAtCursor;
        }

        internal static bool IsCursorAtTestMethod(this TextDocument _textDocument)
        {
            TextPoint textPoint = null;
            CodeElement functionAtCursor = _textDocument.GetFunctionAtCursor(out textPoint);
            if (functionAtCursor == null)
                return false;
            return functionAtCursor.IsAttributeExist("TestMethod");
        }

        internal static CodeElement GetClassElement(this TextDocument _textDocument)
        {
            CodeElement classElement = null;
            try
            {
                TextSelection sel = (TextSelection)_textDocument.Selection;
                TextPoint textPoint = (TextPoint)sel.ActivePoint;
                classElement = textPoint.get_CodeElement(vsCMElement.vsCMElementClass);
            }
            catch
            {
                classElement = null;
            }
            return classElement;
        }

        internal static Dictionary<string, string> GetClassAttributes(this TextDocument _textDocument)
        {
            CodeElement classElement = _textDocument.GetClassElement();
            if (classElement == null)
                return null;
            return classElement.GetAttributes();
        }
    }
}
