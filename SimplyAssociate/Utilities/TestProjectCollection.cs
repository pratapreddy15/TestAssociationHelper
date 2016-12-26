using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.SimplyAssociate.Utilities
{
    internal class TestProjectCollection : IEnumerable<TestProject>
    {
        ActiveSolution _solution = null;
        TestProject[] _testProjects;
        public TestProjectCollection(ActiveSolution activeSolution)
        {
            this._solution = activeSolution;
            Projects _projects = this._solution.VsSolution.Projects;
            InitTestProjectCollection(_projects);
        }

        private void InitTestProjectCollection(Projects childProjects)
        {
            List<TestProject> _projects = new List<TestProject>();
            for (int i = 0; i < childProjects.Count; i++)
            {
                TestProject proj = InitTestProject(childProjects.Item(i + 1));
                if (proj != null)
                    _projects.Add(proj);
            }
            _testProjects = _projects.ToArray();   
        }

        private TestProject InitTestProject(EnvDTE.Project project)
        {
            string projectName = project.Name;
            string projectFullName = _solution.ContainingFolder + project.UniqueName;
            XDocument xDoc = XDocument.Load(projectFullName);
            string xmlNamespace = xDoc.Root.Name.NamespaceName;
            XElement xPropertyGroup = xDoc.Root.Element(XName.Get("PropertyGroup", xmlNamespace));
            if (xPropertyGroup == null)
                return null;
            XElement xTestProjectType = xPropertyGroup.Element(XName.Get("TestProjectType", xmlNamespace));
            if (xTestProjectType == null)
                return null;
            string projectType = xTestProjectType.Value;
            return new TestProject
            {
                Name = projectName,
                FullName = projectFullName,
                ContainingFolder = projectFullName.TrimEnd(projectName.ToCharArray()),
                ProjectType = projectType,
                ParentSolution = this._solution
            };
        }

        internal bool IsNotEmpty
        {
            get
            {
                return this._testProjects.Length > 0;
            }
        }

        internal string OutputPathForActiveProject
        {
            get
            {
                try
                {
                    if (_solution == null)
                        throw new ArgumentNullException("_solution");
                    Project activeProject = this._solution.VsAutomation.GetActiveProject();
                    string configurationName = activeProject.ConfigurationManager.ActiveConfiguration.ConfigurationName;
                    string outputFileName = activeProject.Properties.Item("OutputFileName").Value.ToString();
                    string projectDirectoryPath = activeProject.Properties.Item("FullPath").Value.ToString();
                    return projectDirectoryPath +
                        "bin" +
                        System.IO.Path.DirectorySeparatorChar +
                        configurationName +
                        System.IO.Path.DirectorySeparatorChar +
                        outputFileName;
                }
                catch
                {
                    throw;
                }
            }
        }

        internal TestProject this[int index]
        {
            get
            {
                if (index > this._testProjects.Length)
                    throw new ArgumentOutOfRangeException(string.Format(ErrorMessages.TESTPROJECTCOLLECTION_ISEMPTY, this._testProjects.Length, index));
                return this._testProjects[index];
            }
        }

        internal TestProject this[string name]
        {
            get
            {
                return this._testProjects.FirstOrDefault(e => e.Name == name);
            }
        }

        //internal IEnumerator<TestProject> GetEnumerator()
        //{
        //    foreach (TestProject testProject in _testProjects)
        //    {
        //        if (testProject == null)
        //            break;
        //        yield return testProject;
        //    }
        //}

        //System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        //{
        //    return this.GetEnumerator();
        //}

        public IEnumerator<TestProject> GetEnumerator()
        {
            foreach (TestProject testProject in _testProjects)
            {
                if (testProject == null)
                    break;
                yield return testProject;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
