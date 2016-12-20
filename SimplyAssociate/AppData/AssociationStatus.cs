using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SimplyAssociate.AppData
{
    internal class AssociationStatus
    {
        internal const string ASSOCIATING = "Associating...";
        internal const string ERROR = "Error";
        internal const string FAILED = "Failed";
        internal const string PASSED = "Success";
        internal const string LOADEXISTINGASSOCPROGRESS = "{0} of {1} test methods loaded";

        // For existing test associations
        internal const string INPROGRESS = "In Progress";
        internal const string COMPLETED = "Completed";
        internal const string CANCELLED = "Cancelled";
        internal const string CANCELLING = "Cancelling";
    }
}
