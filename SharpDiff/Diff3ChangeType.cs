using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SharpDiff
{
    public enum Diff3ChangeType
    {
        Equal,

        MergeFrom1,

        MergeFrom2,

        MergeFrom1And2,

        MergeFromBase,

        Conflict
    }
}