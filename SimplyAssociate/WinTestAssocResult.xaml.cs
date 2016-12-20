using Microsoft.SimplyAssociate.AppData;
using Microsoft.SimplyAssociate.Utilities;
using System;
using System.Windows.Controls;

namespace Microsoft.SimplyAssociate
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class WinTestAssocResult : UserControl
    {
        Progress<AssociationProgress> progress;
        const string labelStartAssociating = "Start Associating";
        const string labelStopAssociating = "Stop Associating";
        static bool _isAssociationInProgress;
        int countOfProcessedTests;
        int countOfTotalTests;
        static TestClass _activeTestClass;

        public WinTestAssocResult()
        {
            InitializeComponent();
            progress = new Progress<AssociationProgress>();
            progress.ProgressChanged += progress_ProgressChanged;
            _isAssociationInProgress = false;
            this.dataGridTestAssociations.ItemsSource = TestAssociation.QueuedTestAssociations;
        }

        private void ClearList()
        {
            txtAssociationStatus.Text = "No test are in progress";
            TestAssociation.ResetTestAssociationsQueue();
            this.dataGridTestAssociations.ItemsSource = TestAssociation.QueuedTestAssociations;
        }

        internal void BeforeAssociatingTests(bool multipleTestMethods)
        {
            if (multipleTestMethods)
                this.btnStart.Content = labelStopAssociating;
            else
                this.btnStart.IsEnabled = false;

            ClearList();
            _isAssociationInProgress = true;
        }

        void progress_ProgressChanged(object sender, AssociationProgress e)
        {
            countOfProcessedTests = e.CountOfProcessTests;
            countOfTotalTests = e.CountOfTotalTests;
            if (countOfProcessedTests == countOfTotalTests)
            {
                _isAssociationInProgress = false;
                this.btnStart.Content = labelStartAssociating;
                this.btnStart.IsEnabled = true;
            }
            txtAssociationStatus.Text = string.Format("{0} - {1} of {2} test(s) are processed", e.Status, countOfProcessedTests, countOfTotalTests);
        }

        private void btnClearList_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ClearList();
        }

        internal Progress<AssociationProgress> Progress
        {
            get
            {
                return progress;
            }
        }

        internal static TestClass ActiveTestClass
        {
            get
            {
                return _activeTestClass;
            }
            set
            {
                _activeTestClass = value;
            }
        }

        internal static bool IsAssociationInProgress
        {
            get
            {
                return _isAssociationInProgress;
            }
        }

        private void btnStart_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Content.ToString() == labelStartAssociating)
            {
                BeforeAssociatingTests(true);
                ActiveTestClass.AssociateTestMethods(progress);
            }
            else
            {
                TestAssociation.CancelTestAssociations();
                btn.Content = labelStartAssociating;
                _isAssociationInProgress = false;
                txtAssociationStatus.Text = string.Format("{0} - {1} of {2} test(s) are associated", AssociationStatus.CANCELLING, countOfProcessedTests, countOfTotalTests);
            }
        }

        private void AssociationResultWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_isAssociationInProgress)
            {
                btnStart.Content = labelStopAssociating;
            }
        }
    }
}