using Microsoft.SimplyAssociate.AppData;
using Microsoft.SimplyAssociate.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Microsoft.SimplyAssociate
{
    /// <summary>
    /// Interaction logic for WinExistingTestAssoc.xaml
    /// </summary>
    public partial class WinExistingTestAssoc : UserControl
    {
        Progress<AssociationProgress> progress;
        internal const string labelStartLoading = "Start Loading";
        internal const string labelStopLoading = "Stop Loading";
        int countOfProcessedTests;
        int countOfTotalTests;
        static bool _isLoadingInProgress;

        public WinExistingTestAssoc()
        {
            InitializeComponent();
            progress = new Progress<AssociationProgress>();
            progress.ProgressChanged += progress_ProgressChanged;
            this.dataGridExistingTestAssoc.ItemsSource = TestAssociation.ExistingTestAssociations;
            _isLoadingInProgress = false;
        }

        private void ClearList()
        {
            txtLoadAssocations.Text = "No associations loaded";
            TestAssociation.ResetExistingTestAssocationsQueue();
            this.dataGridExistingTestAssoc.ItemsSource = TestAssociation.ExistingTestAssociations;
        }

        internal void BeforeLoadingTestAssociations(bool multipleTestMethods)
        {
            if (multipleTestMethods)
                this.btnStart.Content = labelStopLoading;
            else
                this.btnStart.IsEnabled = false;

            ClearList();
            _isLoadingInProgress = true;
        }

        void progress_ProgressChanged(object sender, AssociationProgress e)
        {
            countOfProcessedTests = e.CountOfProcessTests;
            countOfTotalTests = e.CountOfTotalTests;
            if (countOfProcessedTests == countOfTotalTests)
            {
                _isLoadingInProgress = false;
                this.btnStart.Content = labelStartLoading;
                this.btnStart.IsEnabled = true;
            }
            txtLoadAssocations.Text = string.Format("{0} - {1} of {2} associations loaded", e.Status, countOfProcessedTests, countOfTotalTests);
        }

        private void btnClearList_Click(object sender, RoutedEventArgs e)
        {
            ClearList();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Content.ToString() == labelStartLoading)
            {
                BeforeLoadingTestAssociations(true);
                ActiveTestClass.LoadExistingTestAssociationsAsync(progress);
            }
            else
            {
                TestAssociation.CancelLoadingOfExistingAssocations();
                this.btnStart.Content = labelStartLoading;
                _isLoadingInProgress = false;
                txtLoadAssocations.Text = string.Format("{0} - {1} of {2} associations loaded", AssociationStatus.CANCELLING, countOfProcessedTests, countOfTotalTests);
            }
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
            get;
            set;
        }

        internal static bool IsLoadingInProgress
        {
            get
            {
                return _isLoadingInProgress;
            }
        }

        private void ExistingTestAssocWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoadingInProgress)
            {
                btnStart.Content = labelStopLoading;
            }
        }
    }
}
