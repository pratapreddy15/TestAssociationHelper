// Guids.cs
// MUST match guids.h
using System;

namespace Microsoft.SimplyAssociate
{
    static class GuidList
    {
        public const string guidSimplyAssociatePkgString = "2524ce99-db9b-4907-aa04-6378e6bafa5e";
        public const string guidSimplyAssociateCmdSetString = "eda0c8f7-1098-41ed-b870-13d4fc910438";
        public const string guidToolWindowPersistanceString = "4bbc7c06-44d8-4353-88c1-5cb54c0bed30";

        public static readonly Guid guidSimplyAssociateCmdSet = new Guid(guidSimplyAssociateCmdSetString);
    };
}