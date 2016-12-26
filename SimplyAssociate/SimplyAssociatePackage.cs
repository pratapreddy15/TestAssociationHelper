using EnvDTE;
using Microsoft.SimplyAssociate.Utilities;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Controls;

namespace Microsoft.SimplyAssociate
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    // This attribute registers a tool window exposed by this package.
    [Guid(GuidList.guidSimplyAssociatePkgString)]
    [ProvideToolWindow(typeof(AssociationResultWindow), Style = VsDockStyle.Float)]
    [ProvideToolWindow(typeof(ExistingTestAssocWindow), Style = VsDockStyle.Float)]
    public sealed class SimplyAssociatePackage : Package
    {
        TestManager _testManager = null;
        ActiveSolution _activeSolution = null;
        MessageBox _msgBox = null;
        bool isCommandInitialized = false;
        bool canInitializeCommands = false;
        bool isBuildRunning = false;

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public SimplyAssociatePackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        /// <summary>
        /// This function is called when the user clicks the menu item that shows the 
        /// tool window. See the Initialize method to see how the menu item is associated to 
        /// this function using the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void ShowToolWindow(object sender, EventArgs e)
        {
            MenuCommand menuCommand = sender as MenuCommand;
            if (menuCommand.CommandID.ID == (int)PkgCmdIDList.cmdWndSimplyAssociationResult)
                ShowToolWindow<AssociationResultWindow>(); // Window for viewing test association results
            else if (menuCommand.CommandID.ID == (int)PkgCmdIDList.cmdWndExistingTestAssociations)
                ShowToolWindow<ExistingTestAssocWindow>(); // Window for viewing existing test associations
        }

        private void ShowToolWindow<T>() where T : ToolWindowPane
        {
            // Get the instance number 0 of this tool window.
            // This window is single instance so this instance is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = this.FindToolWindow(typeof(T), 0, true);
            if ((null == window) || (null == window.Frame))
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            InitTestClassForToolWindows();
            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            windowFrame.Show();
        }

        private T1 GetWindowInstance<T, T1>()
            where T : ToolWindowPane
            where T1 : UserControl
        {
            ToolWindowPane window = this.FindToolWindow(typeof(T), 0, false);
            object content = window.Content;
            if (content is T1)
                return content as T1;
            return null;
        }

        private void CloseToolWindow()
        {
            #region Close the Test Association Results window

            ToolWindowPane window = this.FindToolWindow(typeof(AssociationResultWindow), 0, true);
            if (window != null)
            {
                using (Microsoft.VisualStudio.Platform.WindowManagement.WindowFrame frame = window.Frame as Microsoft.VisualStudio.Platform.WindowManagement.WindowFrame)
                {
                    if (frame.Visible)
                        (window.GetIVsWindowPane() as IVsWindowPane).ClosePane();
                }
            }

            #endregion

            #region Close the Existing Test Associations window

            ToolWindowPane window1 = this.FindToolWindow(typeof(ExistingTestAssocWindow), 0, true);
            if (window1 != null)
            {
                using (Microsoft.VisualStudio.Platform.WindowManagement.WindowFrame frame = window1.Frame as Microsoft.VisualStudio.Platform.WindowManagement.WindowFrame)
                {
                    if (frame.Visible)
                        (window.GetIVsWindowPane() as IVsWindowPane).ClosePane();
                }
            }

            #endregion
        }

        private Progress<AssociationProgress> InitProgressForLoadingTestAssociations(WinExistingTestAssoc winExistingTestAssoc)
        {
            Progress<AssociationProgress> progressOfAssociations = null;
            if (winExistingTestAssoc != null)
                progressOfAssociations = winExistingTestAssoc.Progress;
            return progressOfAssociations;
        }

        private void LoadTestAssociations(WinExistingTestAssoc winExistingTestAssoc, TestClass testClass)
        {
            Progress<AssociationProgress> progressOfAssociations = InitProgressForLoadingTestAssociations(winExistingTestAssoc);
            winExistingTestAssoc.BeforeLoadingTestAssociations(true);
            testClass.LoadExistingTestAssociationsAsync(progressOfAssociations);
        }

        private void LoadTestAssociations(WinExistingTestAssoc winExistingTestAssoc, TestMethod testMethod)
        {
            Progress<AssociationProgress> progressOfAssociations = InitProgressForLoadingTestAssociations(winExistingTestAssoc);
            winExistingTestAssoc.BeforeLoadingTestAssociations(false);
            testMethod.LoadExistingTestAssociationAsync(progressOfAssociations);
        }

        private Progress<AssociationProgress> InitProgressForAssociatingTests(WinTestAssocResult winTestAssociationResult)
        {
            Progress<AssociationProgress> progressOfAssociations = null;
            if (winTestAssociationResult != null)
                progressOfAssociations = winTestAssociationResult.Progress;
            return progressOfAssociations;
        }

        private void AssociateTests(WinTestAssocResult winTestAssocationResult, TestClass testClass)
        {

            Progress<AssociationProgress> progressOfAssociations = InitProgressForAssociatingTests(winTestAssocationResult);
            winTestAssocationResult.BeforeAssociatingTests(true);
            testClass.AssociateTestMethods(progressOfAssociations);
        }

        private void AssociateTests(WinTestAssocResult winTestAssocationResult, TestMethod testMethod)
        {
            Progress<AssociationProgress> progressOfAssociations = InitProgressForAssociatingTests(winTestAssocationResult);
            winTestAssocationResult.BeforeAssociatingTests(true);
            testMethod.Associate(progressOfAssociations);
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            isCommandInitialized = false;
            InitializeCommandHelper();
            InitializeCommands();
        }

        void menuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand menucommand = sender as OleMenuCommand;
            if (!_activeSolution.IsTestClassActive)
            {
                /* Do not show the menu command if user brings up the context menu outside of the test class 
                 * or if the acitve document does not contain a test class */
                menucommand.Visible = false;
                return;
            }
            else if (menucommand.CommandID.ID == PkgCmdIDList.cmdViewAssociatedTest)
            {
                /* Disable the menu command if there is a task for associating the testmethod 
                 * already running */
                menucommand.Enabled = !WinExistingTestAssoc.IsLoadingInProgress;
                return;
            }
            else if (menucommand.CommandID.ID == PkgCmdIDList.cmdAssociateWithTestCase)
            {
                /* Disable the menu command if there is a task for associating the testmethod 
                 * already running */
                menucommand.Enabled = !WinTestAssocResult.IsAssociationInProgress;
                return;
            }

            // Show the menu command in the context menu
            menucommand.Visible = true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            string projectOutputFilePath = _activeSolution.TestProjects.OutputPathForActiveProject;
            if (System.IO.File.Exists(projectOutputFilePath) == false)
            {
                _msgBox.ShowMessage("File Not Found", ErrorMessages.ACTIVEPROJECT_OUTPUTFILE_NOTFOUND, MessageBox.MessageType.ERROR);
                return;
            }

            Utilities.TestClass testClass = _activeSolution.ActiveTestClass;
            if (testClass == null)
                _msgBox.ShowMessage("Unable to Read TetMethods", ErrorMessages.CURSOR_NOTINSIDE_TESTCLASS, MessageBox.MessageType.ERROR);
            else if (isBuildRunning)
                _msgBox.ShowMessage("Build in Progress", ErrorMessages.CANNOTSTART_TESTASSOCIATION_BUILD_INPROGRESS, MessageBox.MessageType.INFORMATION);
            else
            {
                Microsoft.SimplyAssociate.Utilities.TestMethod testMethod = testClass.ActiveTestMethod;
                ShowToolWindow<AssociationResultWindow>();
                WinTestAssocResult winTestAssociationResult = GetWindowInstance<AssociationResultWindow, WinTestAssocResult>();
                if (testMethod == null) // Associate all test methods in current test class
                {
                    AssociateTests(winTestAssociationResult, testClass);
                }
                else // Associate the specific test method
                {
                    AssociateTests(winTestAssociationResult, testMethod);
                }
            }
        }

        private void MenuItemCallForViewTestAssociation(object sender, EventArgs e)
        {
            Utilities.TestClass testClass = _activeSolution.ActiveTestClass;
            if (testClass == null)
                _msgBox.ShowMessage("Unable to Read TetMethods", ErrorMessages.CURSOR_NOTINSIDE_TESTCLASS, MessageBox.MessageType.ERROR);
            else
            {
                TestMethod testMethod = testClass.ActiveTestMethod;
                ShowToolWindow<ExistingTestAssocWindow>();
                WinExistingTestAssoc winExistingTestAssoc = GetWindowInstance<ExistingTestAssocWindow, WinExistingTestAssoc>();
                if (testMethod == null)
                    LoadTestAssociations(winExistingTestAssoc, testClass);
                else
                    LoadTestAssociations(winExistingTestAssoc, testMethod);
            }
        }

        private void InitTestClassForToolWindows()
        {
            TestClass testClass = _activeSolution.ActiveTestClass;
            ToolWindowPane winExistingTestAssoc = this.FindToolWindow(typeof(ExistingTestAssocWindow), 0, false);
            if (winExistingTestAssoc != null)
                WinExistingTestAssoc.ActiveTestClass = testClass;

            ToolWindowPane winAssociateTest = this.FindToolWindow(typeof(AssociationResultWindow), 0, false);
            if (winAssociateTest != null)
                WinTestAssocResult.ActiveTestClass = testClass;
        }

        private void InitializeCommands()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            if (canInitializeCommands == false)
            {
                Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Cannot initialize commands because either the opened solution does not contains test project or it is not available in TFS"));
                return;
            }
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for the menu item Associate with Test Case.
                CommandID menuCommandID = new CommandID(GuidList.guidSimplyAssociateCmdSet, (int)PkgCmdIDList.cmdAssociateWithTestCase);
                OleMenuCommand menuItem = new OleMenuCommand(MenuItemCallback, menuCommandID);
                menuItem.BeforeQueryStatus += menuItem_BeforeQueryStatus;
                mcs.AddCommand(menuItem);

                // Create the command for the menu item View Test Associations.
                CommandID menuCommandIDViewAllAssoc = new CommandID(GuidList.guidSimplyAssociateCmdSet, (int)PkgCmdIDList.cmdViewAssociatedTest);
                OleMenuCommand menuItemForViewAllAssoc = new OleMenuCommand(MenuItemCallForViewTestAssociation, menuCommandIDViewAllAssoc);
                menuItemForViewAllAssoc.BeforeQueryStatus += menuItem_BeforeQueryStatus;
                mcs.AddCommand(menuItemForViewAllAssoc);

                // Create the command for the tool window
                CommandID toolwndCommandID = new CommandID(GuidList.guidSimplyAssociateCmdSet, (int)PkgCmdIDList.cmdWndSimplyAssociationResult);
                MenuCommand menuToolWin = new MenuCommand(ShowToolWindow, toolwndCommandID);
                mcs.AddCommand(menuToolWin);

                // Create the command for the tool window "Existing Test Associations"
                CommandID toolwndExistingTestAssoc = new CommandID(GuidList.guidSimplyAssociateCmdSet, (int)PkgCmdIDList.cmdWndExistingTestAssociations);
                MenuCommand menuToolWinExistingTestAssocs = new MenuCommand(ShowToolWindow, toolwndExistingTestAssoc);
                mcs.AddCommand(menuToolWinExistingTestAssocs);

                isCommandInitialized = true;
                this._activeSolution.VsAutomation.Events.BuildEvents.OnBuildBegin += BuildEvents_OnBuildBegin;
                this._activeSolution.VsAutomation.Events.BuildEvents.OnBuildDone += BuildEvents_OnBuildDone;
            }
        }

        void BuildEvents_OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
        {
            isBuildRunning = false;
            WinTestAssocResult.IsBuildRunning = false;
        }

        void BuildEvents_OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            if (TestAssociation.IsTestAssociationTaskRunning)
            {
                _msgBox.ShowMessage("Cannot Perform Action", ErrorMessages.CANNOTBUILD_TESTASSOCIATIONS_INPROGRESS, MessageBox.MessageType.INFORMATION);
                this._activeSolution.VsAutomation.ExecuteCommand("Build.Cancel");
                isBuildRunning = false;
                WinTestAssocResult.IsBuildRunning = false;
                return;
            }
            isBuildRunning = true;
            WinTestAssocResult.IsBuildRunning = true;
        }

        private void InitializeCommandHelper()
        {
            _activeSolution = new ActiveSolution(this);
            bool containsTestProject = _activeSolution.ContainsTestProject;
            _testManager = new TestManager(_activeSolution);
            _testManager.InitTeamProject();
            bool isTfsProject = _testManager.IsTfsProject;
            if (containsTestProject && isTfsProject)
                canInitializeCommands = true;
            else
                return;
            _msgBox = new MessageBox(GetService(typeof(IVsUIShell)) as IVsUIShell);
        }

        private void HandleSolutionCloseEvent()
        {
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (mcs != null)
            {
                // Remove command Associate with Test Case.
                CommandID menuCommandID = new CommandID(GuidList.guidSimplyAssociateCmdSet, (int)PkgCmdIDList.cmdAssociateWithTestCase);
                MenuCommand menuComm = mcs.FindCommand(menuCommandID);
                if (menuComm != null)
                    mcs.RemoveCommand(menuComm);

                // Remove command View Test Associations.
                CommandID menuCommandIDViewAllAssoc = new CommandID(GuidList.guidSimplyAssociateCmdSet, (int)PkgCmdIDList.cmdViewAssociatedTest);
                MenuCommand menuCommForViewAllAssoc = mcs.FindCommand(menuCommandIDViewAllAssoc);
                if (menuCommForViewAllAssoc != null)
                    mcs.RemoveCommand(menuCommForViewAllAssoc);

                // Remove the tool window Test Association Result.
                CommandID toolwndCommandID = new CommandID(GuidList.guidSimplyAssociateCmdSet, (int)PkgCmdIDList.cmdWndSimplyAssociationResult);
                //MenuCommand menuToolWin = new MenuCommand(ShowToolWindow, toolwndCommandID);
                MenuCommand toolWindowCommand = mcs.FindCommand(toolwndCommandID);
                if (toolWindowCommand != null)
                    mcs.RemoveCommand(toolWindowCommand);

                // Remove the tool window Existing Test Associations
                CommandID toolWndExistingTestAssocs = new CommandID(GuidList.guidSimplyAssociateCmdSet, (int)PkgCmdIDList.cmdWndExistingTestAssociations);
                //MenuCommand menuToolWinExistingTestAssocs = new MenuCommand(ShowToolWindow, toolWndExistingTestAssocs);
                MenuCommand toolWindowViewExistingAssocs = mcs.FindCommand(toolWndExistingTestAssocs);
                if (toolWindowViewExistingAssocs != null)
                    mcs.RemoveCommand(toolWindowViewExistingAssocs);

                isCommandInitialized = false;
                canInitializeCommands = false;
                CloseToolWindow();
                this._activeSolution.VsAutomation.Events.BuildEvents.OnBuildBegin -= BuildEvents_OnBuildBegin;
                TestAssociation.CancelLoadingOfExistingAssocations();
                TestAssociation.CancelTestAssociations();
                TestAssociation.ResetExistingTestAssocationsQueue();
                TestAssociation.ResetTestAssociationsQueue();
            }
        }

        private void HandleSolutionOpenEvent()
        {
            //InitializeCommandHelper();
            if (!isCommandInitialized)
            {
                InitializeCommandHelper();
                InitializeCommands();
            }
        }

        #endregion

        #region Methods visible in package

        internal void HandleSolutionEvent(string eventName)
        {
            if (eventName == "OnBeforeCloseSolution")
                HandleSolutionCloseEvent();

            if (eventName == "OnAfterOpenSolution")
                HandleSolutionOpenEvent();
        }

        internal T GetService<T>() where T : class
        {
            return GetService(typeof(T)) as T;
        }

        #endregion
    }
}
