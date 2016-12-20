using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SimplyAssociate.Utilities
{
    internal class MessageBox
    {
        internal enum MessageType
        {
            INFORMATION,
            ERROR,
            WARNING
        }

        IVsUIShell uiShell = null;

        public MessageBox(IVsUIShell _vsUIShell)
        {
            this.uiShell = _vsUIShell;
        }

        public void ShowMessage(string title, string message, MessageType messageType)
        {
            Guid clsid = Guid.Empty;
            int result;
            OLEMSGICON msgIcon = OLEMSGICON.OLEMSGICON_NOICON;
            switch (messageType)
            {
                case MessageType.INFORMATION:
                    msgIcon = OLEMSGICON.OLEMSGICON_INFO;
                    break;
                case MessageType.WARNING:
                    msgIcon = OLEMSGICON.OLEMSGICON_WARNING;
                    break;
                case MessageType.ERROR:
                    msgIcon = OLEMSGICON.OLEMSGICON_CRITICAL;
                    break;
            }
            uiShell.ShowMessageBox(0, ref clsid, title, message, null, 0, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, msgIcon, 0, out result);
        }
    }
}
