// PkgCmdID.cs
// MUST match PkgCmdID.h
using System;

namespace Microsoft.SimplyAssociate
{
    static class PkgCmdIDList
    {
        public const uint cmdAssociateWithTestCase = 0x100;
        public const uint cmdWndSimplyAssociationResult = 0x101;
        public const uint cmdViewAssociatedTest = 0x102;
        public const uint cmdWndExistingTestAssociations = 0x103;
    };
}