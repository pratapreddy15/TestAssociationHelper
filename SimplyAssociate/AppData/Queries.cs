using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SimplyAssociate.AppData
{
    internal class Queries
    {
        internal const string SELECT_TESTCASE = "SELECT [Id], [Title] FROM WorkItem WHERE [Id] = {0}";
    }
}
