using EnvDTE;
using Microsoft.SimplyAssociate.AppData;
using System;
using System.Collections.Generic;

namespace Microsoft.SimplyAssociate.Utilities
{
    internal class TestMethod
    {
        System.Threading.CancellationTokenSource cts_ExistingAssociation;
        System.Threading.CancellationTokenSource cts_AssociateTestMethods;

        TestClass parentTestClass;
        CodeElement _testMethod;
        System.Threading.Tasks.Task task_LoadingExistingTestAssoc;

        //internal enum TaskTypes
        //{
        //    ASSOCIATETEST,
        //    LOADEXISTINGTESTASSOC
        //}

        /// <summary>
        /// Use this constructor if the specific test method is known. For e.g. user is iterating through test methods in a test class.
        /// </summary>
        /// <param name="testMethod">The method in test class having attribute TestMethod.</param>
        /// <param name="nameOfTestClass">The name of the containing test class in which the test method is present.</param>
        internal TestMethod(TestClass testClass, CodeElement testMethod)
        {
            this._testMethod = testMethod;
            this.WorkItemId = testMethod.GetAttributeValue("WorkItem");
            this.Name = testMethod.Name;
            this.FullName = testMethod.FullName;
            this.parentTestClass = testClass;
        }

        /// <summary>
        /// Use this constructor if the specific test method in context is unkonwn. For e.g. user has right clicked on a test method but not sure which test method.
        /// </summary>
        /// <param name="testMethod">The method in test class having attribute TestMethod.</param>
        /// <param name="pointAtCursor">The cursor or a point on the document at which the user has performed the right click operation.</param>
        /// <param name="nameOfTestClass">The name of the containing test class in which the test method is present.</param>
        internal TestMethod(TestClass testClass, CodeElement testMethod, TextPoint pointAtCursor)
        {
            this.parentTestClass = testClass;
            this._testMethod = testMethod;
            int activeLineNumber = pointAtCursor.Line;
            CodeElements allFunctions = testMethod.Collection;
            foreach (CodeElement currElem in allFunctions)
            {
                int startLine = currElem.StartPoint.Line;
                int endLine = currElem.EndPoint.Line;
                if ((activeLineNumber >= startLine && activeLineNumber <= endLine))
                {
                    this.WorkItemId = testMethod.GetAttributeValue("WorkItem");
                    this.Name = currElem.Name;
                    this.FullName = currElem.FullName;
                    break;
                }
            }
        }

        internal string WorkItemId
        {
            get;
            private set;
        }

        internal TestClass TestClass
        {
            get
            {
                return this.parentTestClass;
            }
        }

        internal string TestClassName
        {
            get
            {
                return this.parentTestClass.Name;
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

        internal AssociationInfo AssocationInfo
        {
            get
            {
                return new AssociationInfo { TestCaseId = this.WorkItemId, TestMethodName = this.Name };
            }
        }

        internal async void Associate(Progress<AssociationProgress> associationProgress)
        {
            cts_AssociateTestMethods = new System.Threading.CancellationTokenSource();
            await TestAssociation.AssociateTestMethodsAsync(new TestMethod[] { this }, new System.Threading.CancellationTokenSource(), associationProgress);
        }

        internal async void LoadExistingTestAssociationAsync(Progress<AssociationProgress> associationProgress)
        {
            cts_ExistingAssociation = new System.Threading.CancellationTokenSource();
            await TestAssociation.AddToExistingTestAssociationAsync(new TestMethod[] { this }, cts_ExistingAssociation, associationProgress);
        }
    }
}
