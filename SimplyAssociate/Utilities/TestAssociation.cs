using Microsoft.SimplyAssociate.AppData;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace Microsoft.SimplyAssociate.Utilities
{
    internal class TestAssociation
    {
        internal static ObservableCollection<AssociationInfo> QueuedTestAssociations = new ObservableCollection<AssociationInfo>();
        internal static ObservableCollection<ExistingAssociationInfo> ExistingTestAssociations = new ObservableCollection<ExistingAssociationInfo>();
        static System.Threading.CancellationTokenSource tokenForExistingAssocationTask = new System.Threading.CancellationTokenSource();
        static System.Threading.CancellationTokenSource tokenForAssociateTestTask = new System.Threading.CancellationTokenSource();

        internal static void AssociateTestMethod(AssociationInfo assocInfo, TestProject testProject, TestClass testClass)
        {
            if (TestManager.teamProject == null)
                throw new ArgumentNullException(ErrorMessages.TEAMPROJECT_ISNULL);

            string assemblyFullPath = testProject.OutputFilePath,
                testProjectType = testProject.ProjectType,
                testClassName = testClass.FullName;

            assocInfo.Status = AssociationStatus.ASSOCIATING;
            assocInfo.ImagePath = "";
            string tcId = assocInfo.TestCaseId;
            string errorMessage = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(tcId))
                {
                    assocInfo.Status = AssociationStatus.ERROR;
                    assocInfo.ImagePath = FilePath.IMAGE_ERROR;
                    assocInfo.ErrorMessage = string.Format(ErrorMessages.TFS_WORKITEM_NOTFOUND_FOR_TESTMETHOD,
                        assocInfo.TestMethodName);
                }
                else
                {
                    IList<ITestCase> tfsTestCases = TestManager.teamProject.TestCases.Query(string.Format(Queries.SELECT_TESTCASE, tcId)).ToList();
                    if (tfsTestCases.Count > 1)
                    {
                        assocInfo.Status = AssociationStatus.FAILED;
                        assocInfo.ImagePath = FilePath.IMAGE_FAILED;
                        assocInfo.ErrorMessage = string.Format(ErrorMessages.TFS_MORETHANONE_TESTCASE_FOUND, assocInfo.TestCaseId.ToString());
                    }
                    else
                    {
                        #region Association of test case with test method happens here

                        ITestCase tfsTestCase = tfsTestCases[0];
                        Assembly testAssembly = Assembly.LoadFrom(assemblyFullPath);
                        Type classType = testAssembly.GetType(testClassName, true);
                        MethodInfo testMethod = classType.GetMethod(assocInfo.TestMethodName);
                        string testMethodFullName = string.Concat(testMethod.DeclaringType.FullName, ".", testMethod.Name);
                        byte[] bytesTestMethodFullName = System.Text.Encoding.Unicode.GetBytes(testMethodFullName);
                        SHA256 cryptoService = SHA256CryptoServiceProvider.Create();
                        byte[] hashedTestMethod = cryptoService.ComputeHash(bytesTestMethodFullName);
                        byte[] generatedGuid = new byte[16]; // Byte array for GUID must be exactly 16 bytes long
                        Array.Copy(hashedTestMethod, generatedGuid, 16);
                        Guid guidOfTestMethod = new Guid(generatedGuid);

                        ITmiTestImplementation testImplementation = TestManager.teamProject.CreateTmiTestImplementation(
                            testMethodFullName,
                            testProjectType,
                            testAssembly.GetName().Name,
                            guidOfTestMethod);
                        try
                        {
                            tfsTestCase.WorkItem.Open();
                            foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Field item in tfsTestCase.WorkItem.Fields)
                            {
                                if (item.Name == "Automation Complexity")
                                {
                                    string allowedValues = string.Join(", ", item.AllowedValues.Cast<string>());
                                    System.Diagnostics.Debug.WriteLine(string.Format("Name: {0}; Allowed Values: {1}", item.Name, allowedValues));
                                }
                            }
                            tfsTestCase.Implementation = testImplementation;
                            tfsTestCase.Save();
                            string statusAfterAssociation = tfsTestCase.CustomFields["Microsoft.VSTS.TCM.AutomationStatus"].Value.ToString();
                            assocInfo.AutomationStatus = statusAfterAssociation;
                            tfsTestCase.WorkItem.Close();
                            assocInfo.Status = AssociationStatus.PASSED;
                            assocInfo.ImagePath = FilePath.IMAGE_PASSED;
                        }
                        catch
                        {
                            if (tfsTestCase.WorkItem.IsOpen)
                                tfsTestCase.WorkItem.Close();
                            throw;
                        }

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                assocInfo.Status = AssociationStatus.ERROR;
                assocInfo.ImagePath = FilePath.IMAGE_ERROR;
                assocInfo.ErrorMessage = ex.Message;
            }
        }

        internal static Dictionary<string, string> GetAssociatedTest(string workItemId)
        {
            if (TestManager.teamProject == null)
                throw new ArgumentNullException(ErrorMessages.TEAMPROJECT_ISNULL);

            string automationStatus = string.Empty,
                automationTestName = string.Empty,
                automationTestStorage = string.Empty,
                automationTestType = string.Empty;
            string[] testImplementation;

            // Microsoft.TeamFoundation.TeamFoundationServiceUnavailableException
            IList<ITestCase> tfsTestCases = null;
            try
            {
                tfsTestCases = TestManager.teamProject.TestCases.Query(string.Format(Queries.SELECT_TESTCASE, workItemId)).ToList();
            }
            catch
            {
                throw;
            }
            if (tfsTestCases.Count > 1)
                throw new InvalidOperationException(string.Format(ErrorMessages.TFS_MORETHANONE_TESTCASE_FOUND, workItemId));
            ITestCase testCase = tfsTestCases[0];
            automationStatus = testCase.CustomFields["Microsoft.VSTS.TCM.AutomationStatus"].Value.ToString();
            ITestImplementation testImp = testCase.Implementation;
            if (testImp != null)
                testImplementation = testImp.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            else
                testImplementation = new string[0];
            if (testImplementation.Length > 0)
            {
                automationTestName = testImplementation[1];
                automationTestStorage = testImplementation[2];
                automationTestType = testImplementation[3];
            }

            return new Dictionary<string, string>
            {
                { TestImplementationFields.AUTOMATIONSTATUS, automationStatus },
                { TestImplementationFields.AUTOMATEDTESTNAME, automationTestName },
                { TestImplementationFields.AUTOMATEDTESTSTORAGE, automationTestStorage },
                { TestImplementationFields.AUTOMATEDTESTTYPE, automationTestType }
            };
        }

        internal static System.Threading.Tasks.Task AssociateTestMethodsAsync(TestMethod[] testMethods,
            System.Threading.CancellationTokenSource tokenSource, IProgress<AssociationProgress> progressOfAssociations)
        {
            tokenForAssociateTestTask = tokenSource;
            return System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                int countOfTotalTests = testMethods.Length;
                int countOfProcessedTests = 0;
                AssociationProgress assocProgress = new AssociationProgress
                {
                    CountOfTotalTests = countOfTotalTests,
                    CountOfProcessTests = countOfProcessedTests,
                    Status = AssociationStatus.INPROGRESS
                };
                progressOfAssociations.Report(assocProgress);

                foreach (TestMethod testMethod in testMethods)
                {
                    if (tokenForAssociateTestTask.IsCancellationRequested)
                    {
                        assocProgress.Status = AssociationStatus.CANCELLED;
                        progressOfAssociations.Report(assocProgress);
                        return;
                    }
                    AssociationInfo assocInfo = testMethod.AssocationInfo;
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            QueuedTestAssociations.Add(assocInfo);
                        });
                    AssociateTestMethod(assocInfo, testMethod.TestClass.ParentTestProject, testMethod.TestClass);
                    assocProgress.CountOfProcessTests = ++countOfProcessedTests;
                    if (countOfProcessedTests == countOfTotalTests)
                        assocProgress.Status = AssociationStatus.COMPLETED;
                    progressOfAssociations.Report(assocProgress);
                }
            }, tokenForAssociateTestTask.Token);
        }

        internal static System.Threading.Tasks.Task AddToExistingTestAssociationAsync(TestMethod[] testMethods,
            System.Threading.CancellationTokenSource tokenSource, IProgress<AssociationProgress> progressOfAssocations)
        {
            tokenForExistingAssocationTask = tokenSource;
            return System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    int countOfTotalTests = testMethods.Length;
                    int countOfProcessTests = 0;
                    AssociationProgress assocProgress = new AssociationProgress
                    {
                        CountOfTotalTests = countOfTotalTests,
                        CountOfProcessTests = countOfProcessTests,
                        Status = AssociationStatus.INPROGRESS
                    };
                    progressOfAssocations.Report(assocProgress);
                    foreach (TestMethod testMethod in testMethods)
                    {
                        try
                        {
                            if (tokenForExistingAssocationTask.IsCancellationRequested)
                            {
                                assocProgress.Status = AssociationStatus.CANCELLED;
                                progressOfAssocations.Report(assocProgress);
                                return;
                            }
                            ExistingAssociationInfo testAssocationInfo = new ExistingAssociationInfo();
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                ExistingTestAssociations.Add(testAssocationInfo);
                            });
                            Dictionary<string, string> associatedTestData = GetAssociatedTest(testMethod.WorkItemId);
                            testAssocationInfo.TestCaseId = testMethod.WorkItemId;
                            testAssocationInfo.TestClass = testMethod.TestClassName;
                            testAssocationInfo.TestMethod = testMethod.Name;
                            testAssocationInfo.AutomationStatus = associatedTestData[TestImplementationFields.AUTOMATIONSTATUS];
                            testAssocationInfo.AutomatedTestName = associatedTestData[TestImplementationFields.AUTOMATEDTESTNAME];
                            testAssocationInfo.AutomatedTestStorage = associatedTestData[TestImplementationFields.AUTOMATEDTESTSTORAGE];
                            testAssocationInfo.AutomatedTestType = associatedTestData[TestImplementationFields.AUTOMATEDTESTTYPE];
                        }
                        catch (OperationCanceledException)
                        {
                            assocProgress.Status = AssociationStatus.CANCELLED;
                            progressOfAssocations.Report(assocProgress);
                            return;
                        }

                        assocProgress.CountOfProcessTests = ++countOfProcessTests;
                        if (countOfProcessTests == countOfTotalTests)
                            assocProgress.Status = AssociationStatus.COMPLETED;
                        progressOfAssocations.Report(assocProgress);
                    }
                }, tokenForExistingAssocationTask.Token);
        }

        internal static void CancelLoadingOfExistingAssocations()
        {
            if (tokenForExistingAssocationTask != null)
                tokenForExistingAssocationTask.Cancel();
        }

        internal static void CancelTestAssociations()
        {
            if (tokenForAssociateTestTask != null)
                tokenForAssociateTestTask.Cancel();
        }

        internal static void ResetTestAssociationsQueue()
        {
            QueuedTestAssociations.Clear();
            QueuedTestAssociations = new ObservableCollection<AssociationInfo>();
        }

        internal static void ResetExistingTestAssocationsQueue()
        {
            ExistingTestAssociations.Clear();
            ExistingTestAssociations = new ObservableCollection<ExistingAssociationInfo>();
        }
    }
}
