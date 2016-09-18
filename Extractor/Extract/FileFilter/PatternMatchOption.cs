using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extractor.Extract
{
    /// <summary>
    /// Item pattern match option
    /// </summary>
    public enum PatternMatchOption
    {
        /// <summary>
        /// Match item pattern as the start of the item.
        /// </summary>
        MatchAsStart,

        /// <summary>
        /// Match item pattern as the end of the item.
        /// </summary>
        MatchAsEnd
    }
}
