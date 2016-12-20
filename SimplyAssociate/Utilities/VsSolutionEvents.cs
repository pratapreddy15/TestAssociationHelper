using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SimplyAssociate.Utilities
{
    internal class VsSolutionEvents : IVsSolutionEvents
    {
        private SimplyAssociatePackage vsPackage;

        public VsSolutionEvents(SimplyAssociatePackage package)
        {
            vsPackage = package;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            vsPackage.HandleSolutionEvent("OnAfterCloseSolution");
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            vsPackage.HandleSolutionEvent("OnAfterLoadProject");
            return VSConstants.S_OK;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            vsPackage.HandleSolutionEvent("OnAfterOpenProject");
            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            vsPackage.HandleSolutionEvent("OnAfterOpenSolution");
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            vsPackage.HandleSolutionEvent("OnBeforeCloseProject");
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            vsPackage.HandleSolutionEvent("OnBeforeCloseSolution");
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            vsPackage.HandleSolutionEvent("OnBeforeUnloadProject");
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            vsPackage.HandleSolutionEvent("OnQueryCloseProject");
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            vsPackage.HandleSolutionEvent("OnQueryCloseSolution");
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            vsPackage.HandleSolutionEvent("OnQueryUnloadProject");
            return VSConstants.S_OK;
        }
    }
}
