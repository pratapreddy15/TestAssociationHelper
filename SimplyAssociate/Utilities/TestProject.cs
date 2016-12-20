using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SimplyAssociate.Utilities
{
    internal class TestProject
    {
        internal TestProject()
        {
        }

        internal string Name { get; set; }
        internal string FullName { get; set; }
        internal string ContainingFolder { get; set; }
        internal string ProjectType { get; set; }
        internal ActiveSolution ParentSolution
        {
            get;
            set;
        }
        internal string OutputFilePath
        {
            get
            {
                Project activeProject = this.ParentSolution.VsAutomation.GetActiveProject();
                string configurationName = activeProject.ConfigurationManager.ActiveConfiguration.ConfigurationName;
                string outputFileName = activeProject.GetOutputFileName();
                string projectDirectoryPath = activeProject.GetOutputDirectoryPath();
                return projectDirectoryPath +
                    "bin" +
                    System.IO.Path.DirectorySeparatorChar +
                    configurationName +
                    System.IO.Path.DirectorySeparatorChar +
                    outputFileName;
            }
        }
    }
}
