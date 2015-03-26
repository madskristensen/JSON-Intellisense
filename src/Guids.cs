// Guids.cs
// MUST match guids.h
using System;

namespace JSON_Intellisense
{
    static class GuidList
    {
        public const string guidJSON_IntellisensePkgString = "aaa8d5c5-24d8-4c45-9620-9f77b2aa6363";
        public const string guidJSON_IntellisenseCmdSetString = "302cecba-3f94-48f0-ab00-3549d04b6a51";

        public static readonly Guid guidJSON_IntellisenseCmdSet = new Guid(guidJSON_IntellisenseCmdSetString);
    };
}