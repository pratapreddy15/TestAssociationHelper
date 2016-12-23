using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SimplyAssociate.Utilities
{
    [Serializable]
    public class NewTestAssociationInfo
    {
        public string TestName { get; set; }
        public string Storage { get; set; }
        public Guid TestId { get; set; }
    }
}
