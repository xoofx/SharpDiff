using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SharpDiff
{
    /// <summary>
    /// A diff3 change type.
    /// </summary>
    public enum Diff3ChangeType
    {
        /// <summary>
        /// The values are equal between base, v1 and v2.
        /// </summary>
        Equal,

        /// <summary>
        /// The value can be merged from v1.
        /// </summary>
        MergeFrom1,

        /// <summary>
        /// The value can be merged from v2.
        /// </summary>
        MergeFrom2,

        /// <summary>
        /// The value can be merged from v1 and v2.
        /// </summary>
        MergeFrom1And2,

        /// <summary>
        /// The value can be merged from base.
        /// </summary>
        MergeFromBase,

        /// <summary>
        /// The value are different between base, v1 and v2.
        /// </summary>
        Conflict
    }
}