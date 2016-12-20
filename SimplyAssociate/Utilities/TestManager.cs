using Microsoft.SimplyAssociate.AppData;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SimplyAssociate.Utilities
{
    internal class TestManager
    {
        ActiveSolution _activeSolution = null;
        internal static ITestManagementTeamProject teamProject;

        internal TestManager(ActiveSolution activeSolution)
        {
            this._activeSolution = activeSolution;
        }

        internal void InitTeamProject()
        {
            VersionControlExt vcExt = this._activeSolution.VsSolution.DTE.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt") as VersionControlExt;
            string solutionDirectoryPath = this._activeSolution.ContainingFolder;

            TeamFoundation.VersionControl.Client.TeamProject _versionTeamProject = vcExt.SolutionWorkspace.GetTeamProjectForLocalPath(_activeSolution.FullName);
            if (_versionTeamProject == null)
                throw new ArgumentNullException(string.Format(ErrorMessages.TEAMPROJECT_NOTFOUND_ATMAPPEDSOLUTION, solutionDirectoryPath));
            string teamProjectName = _versionTeamProject.Name;
            ITestManagementService testManagementService = _versionTeamProject.TeamProjectCollection.GetService<ITestManagementService>();
            teamProject = testManagementService.GetTeamProject(teamProjectName);
        }
    }
}
