using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Microsoft.SimplyAssociate.Utilities
{
    public class ExistingAssociationInfo : INotifyPropertyChanged
    {
        private string _testCaseId;
        public string TestCaseId
        {
            get
            {
                return _testCaseId;
            }
            set
            {
                if (_testCaseId != value)
                {
                    _testCaseId = value;
                    OnPropertyChanged("TestCaseId");
                }
            }
        }

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

        private string _testClass;
        public string TestClass
        {
            get
            {
                return _testClass;
            }
            set
            {
                if (_testClass != value)
                {
                    _testClass = value;
                    OnPropertyChanged("TestClass");
                }
            }
        }

        private string _testMethod;
        public string TestMethod
        {
            get
            {
                return _testMethod;
            }
            set
            {
                if (_testMethod != value)
                {
                    _testMethod = value;
                    OnPropertyChanged("TestMethod");
                }
            }
        }

        private string _automatedTestName;
        public string AutomatedTestName
        {
            get
            {
                return _automatedTestName;
            }
            set
            {
                if (_automatedTestName != value)
                {
                    _automatedTestName = value;
                    OnPropertyChanged("AutomatedTestName");
                }
            }
        }

        private string _automatedTestStorage;
        public string AutomatedTestStorage
        {
            get
            {
                return _automatedTestStorage;
            }
            set
            {
                if (_automatedTestStorage != value)
                {
                    _automatedTestStorage = value;
                    OnPropertyChanged("AutomatedTestStorage");
                }
            }
        }

        private string _automatedTestType;
        public string AutomatedTestType
        {
            get
            {
                return _automatedTestType;
            }
            set
            {
                if (_automatedTestType != value)
                {
                    _automatedTestType = value;
                    OnPropertyChanged("AutomatedTestType");
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
