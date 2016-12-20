using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SimplyAssociate.Utilities
{
    internal class TestClass
    {
        System.Threading.CancellationTokenSource cts_ExistingAssociations;
        System.Threading.CancellationTokenSource cts_AssociateTestMethod;

        internal enum TestClassTypes
        {
            INVALID,
            UNIT_TEST,
            CODEDUI_TEST
        }

        CodeElement activeTestClass = null;
        TextDocument _activeDocument = null;
        TestProject _parentTestProject;

        internal TestClass(TestProject parentProject, TextDocument  textDocument)
        {
            this._activeDocument = textDocument;
            this._parentTestProject = parentProject;
            InitTestClass(this._activeDocument);
        }

        internal void InitTestClass(TextDocument textDocument)
        {
            TextSelection textSel = textDocument.Selection;
            if (textSel == null)
                throw new ArgumentNullException("textSel");
            TextPoint point = textSel.ActivePoint as TextPoint;
            activeTestClass = point.get_CodeElement(vsCMElement.vsCMElementClass);
            this.Name = this.activeTestClass.Name;
            this.FullName = this.activeTestClass.FullName;
        }

        internal bool IsActive
        {
            get
            {
                CodeElements attributes = ((EnvDTE80.CodeClass2)this.activeTestClass).Attributes;
                return false;
            }
            set
            {
            }
        }

        internal string Name
        {
            get;
            private set;
        }

        internal string FullName
        {
            get;
            private set;
        }

        internal TestProject ParentTestProject
        {
            get
            {
                return this._parentTestProject;
            }
        }

        internal TestClassTypes TestClassType
        {
            get
            {
                Dictionary<string, string> attributes = this._activeDocument.GetClassAttributes();
                bool isUnitTestClass = attributes.Keys.Contains("TestClass");
                bool isCodedUITestClass = attributes.Keys.Contains("CodedUITest");
                if (isUnitTestClass && isCodedUITestClass)
                    throw new Exception(ErrorMessages.CLASS_HAS_TESTCLASS_CODEDUITEST_ATTRIBUTES);
                if (isUnitTestClass)
                    return TestClassTypes.UNIT_TEST;
                if (isCodedUITestClass)
                    return TestClassTypes.CODEDUI_TEST;
                return TestClassTypes.INVALID;
            }
        }

        internal bool IsCursorOnTestMethod
        {
            get
            {
                return this._activeDocument.IsCursorAtTestMethod();
            }
        }

        internal TestMethod ActiveTestMethod
        {
            get
            {
                try
                {
                    TextPoint textPoint;
                    CodeElement functionAtCursor = this._activeDocument.GetFunctionAtCursor(out textPoint);
                    if (functionAtCursor != null && functionAtCursor.IsAttributeExist("TestMethod"))
                        return new TestMethod(this, functionAtCursor, textPoint);
                    return null;
                }
                catch
                {
                    throw;
                }
            }
        }

        internal TestMethod[] TestMethods
        {
            get
            {
                List<TestMethod> testMethods = new List<TestMethod>();
                TextSelection sel = (TextSelection)this._activeDocument.Selection;
                TextPoint pnt = (TextPoint)sel.ActivePoint;
                CodeElements allFunctions = ((EnvDTE80.CodeClass2)(pnt.get_CodeElement(vsCMElement.vsCMElementClass))).Members;
                foreach (CodeElement currElem in allFunctions)
                {
                    if (currElem.Kind == vsCMElement.vsCMElementFunction && currElem.IsAttributeExist("TestMethod"))
                    {
                        testMethods.Add(new TestMethod(this, currElem));
                    }
                }
                return testMethods.ToArray();
            }
        }

        internal TestMethod[] FindTestMethods(params string[] workItemIds)
        {
            List<TestMethod> testMethods = new List<TestMethod>();
            TextSelection sel = (TextSelection)this._activeDocument.Selection;
            TextPoint pnt = (TextPoint)sel.ActivePoint;
            CodeElements allFunctions = ((EnvDTE80.CodeClass2)(pnt.get_CodeElement(vsCMElement.vsCMElementClass))).Members;
            foreach (CodeElement currElem in allFunctions)
            {
                if (currElem.IsAttributeExist("TestMethod") && workItemIds.Contains(currElem.GetAttributeValue("WorkItem")))
                {
                    testMethods.Add(new TestMethod(this, currElem));
                }
            }
            return testMethods.ToArray();
        }

        internal async void LoadExistingTestAssociationsAsync(Progress<AssociationProgress> associationProgress)
        {
            cts_ExistingAssociations = new System.Threading.CancellationTokenSource();
            await TestAssociation.AddToExistingTestAssociationAsync(TestMethods, cts_ExistingAssociations, associationProgress);
        }

        internal async void AssociateTestMethods(Progress<AssociationProgress> associationProgress)
        {
            cts_AssociateTestMethod = new System.Threading.CancellationTokenSource();
            await TestAssociation.AssociateTestMethodsAsync(TestMethods, new System.Threading.CancellationTokenSource(), associationProgress);
        }
    }
}
