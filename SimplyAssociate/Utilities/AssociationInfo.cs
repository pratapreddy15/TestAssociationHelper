using System.ComponentModel;
using System.Windows.Controls;

namespace Microsoft.SimplyAssociate.Utilities
{
    public class AssociationInfo : INotifyPropertyChanged
    {
        public string TestCaseId { get; set; }

        public string TestMethodName { get; set; }

        private string _automationStatus;
        public string AutomationStatus
        {
            get
            {
                return _automationStatus;
            }
            set
            {
                if (_automationStatus != value)
                {
                    _automationStatus = value;
                    OnPropertyChanged("AutomationStatus");
                }
            }
        }

        private string _status;
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged("Status");
                }
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged("ErrorMessage");
                }
            }
        }

        private string _imagePath;
        public string ImagePath
        {
            get
            {
                return _imagePath;
            }
            set
            {
                if (_imagePath != value)
                {
                    _imagePath = value;
                    OnPropertyChanged("ImagePath");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string memberName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(memberName));
            }
        }
    }
}
