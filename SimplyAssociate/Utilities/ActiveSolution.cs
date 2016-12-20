using EnvDTE;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SimplyAssociate.Utilities
{
    internal class ActiveSolution
    {
        SimplyAssociatePackage extensionPackage = null;
        EnvDTE.DTE vsAutomation = null;
        TestProjectCollection testProjects = null;
        IVsSolution vsSolutionAutomation = null;
        VsSolutionEvents _solutionEvents = null;
        uint solutionEventsCookie = 0;

        public ActiveSolution(SimplyAssociatePackage package)
        {
            if (package == null)
                throw new ArgumentNullException(ErrorMessages.VS_EXTENSIONPACKAGE_ISNULL);
            this.extensionPackage = package;
            
            vsAutomation = this.extensionPackage.GetService<EnvDTE.DTE>();
            if (vsAutomation == null)
                throw new ArgumentNullException(ErrorMessages.ENVDTE_ISNULL);
            
            this.vsSolutionAutomation = this.extensionPackage.GetService<SVsSolution>() as IVsSolution;
            this.VsSolution = (EnvDTE80.Solution2)this.vsAutomation.Solution;
            if (this.VsSolution == null)
                throw new ArgumentNullException(ErrorMessages.ACTIVESOLUTION_NOTAVAILABLE);
            this._solutionEvents = new VsSolutionEvents(this.extensionPackage);
            InitPathOfActiveSolution();
            testProjects = new TestProjectCollection(this);
            this.BindEvents();
        }

        private void InitPathOfActiveSolution()
        {
            string pathOfSolution = this.VsSolution.FullName;
            if (pathOfSolution.EndsWith(".sln"))
            {
                string[] splittedPath = pathOfSolution.Split(System.IO.Path.DirectorySeparatorChar);
                pathOfSolution = "";
                for (int i = 0; i < splittedPath.Length - 1; i++)
                    pathOfSolution += splittedPath[i] + System.IO.Path.DirectorySeparatorChar;
            }
            this.ContainingFolder = pathOfSolution;
        }

        private void BindEvents()
        {
            if (_solutionEvents == null)
                throw new ArgumentNullException(ErrorMessages.VS_SOLUTIONEVENTS_ISNULL);
            this.vsSolutionAutomation.AdviseSolutionEvents(_solutionEvents, out solutionEventsCookie);
        }

        internal EnvDTE.DTE VsAutomation
        {
            get
            {
                return this.vsAutomation;
            }
        }

        internal TestProjectCollection TestProjects
        {
            get
            {
                return testProjects;
            }
        }

        internal string FullName
        {
            get
            {
                return this.VsSolution.FullName;
            }
        }

        internal bool ContainsTestProject
        {
            get
            {
                return this.testProjects.IsNotEmpty;
            }
        }

        internal string ContainingFolder
        {
            get;
            private set;
        }

        internal EnvDTE80.Solution2 VsSolution
        {
            get;
            private set;
        }

        internal bool IsActiveDocumentInCSharp
        {
            get
            {
                try
                {
                    return this.vsAutomation.GetActiveTextDocument().Language.ToLowerInvariant() == "csharp";
                }
                catch
                {
                    throw;
                }
            }
        }

        internal bool IsTestClassActive
        {
            get
            {
                if (!IsActiveDocumentInCSharp)
                    return false;
                TextSelection sel = this.vsAutomation.GetActiveTextSelection();
                TextPoint pnt = (TextPoint)sel.ActivePoint;
                CodeElement classElement = pnt.get_CodeElement(vsCMElement.vsCMElementClass);
                if (classElement == null)
                    return false;
                CodeElements classAttributes = ((EnvDTE80.CodeClass2)classElement).Attributes;
                foreach (CodeElement currAttribute in classAttributes)
                {
                    if (currAttribute.Name == "TestClass" || currAttribute.Name == "CodedUITest")
                        return true;
                }
                return false;
            }
        }

        internal TestProject ActiveProject
        {
            get
            {
                try
                {
                    string nameOfActiveProject = this.vsAutomation.GetActiveProject().FullName;
                    foreach (TestProject testProj in this.TestProjects)
                    {
                        if (testProj.FullName == nameOfActiveProject)
                        {
                            return testProj;
                        }
                    }
                    return null;
                }
                catch
                {
                    throw;
                }
            }
        }

        internal TestClass ActiveTestClass
        {
            get
            {
                return new TestClass(this.ActiveProject, this.vsAutomation.GetActiveTextDocument());
            }
        }
    }
}
