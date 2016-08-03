using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extractor.Extract
{
    public abstract class FileFilterBase : IFileFilter
    {
        /// <summary>
        /// Filter our the Increamentation Files details by comparing marker and start time
        /// </summary>
        /// <param name="filesDetail">Files' detail.</param>
        /// <param name="StartTime">date time threshold used to idenfy whether a file is valid and extractable.</param>
        /// <param name="marker">Files marker.</param>
        /// <returns>Extractable files details</returns>
        public abstract IEnumerable<Tuple<DateTime, long, string>> FilterIncreamentationFileDetails(IEnumerable<Tuple<DateTime, long, string>> filesDetail, DateTime StartTime, FileMarkerManager marker);

        /// <summary>
        /// Get files' increament contents by comparing the splitIndex, and split them by splitPattern, then apply data.
        /// </summary>
        /// <param name="filesDetail">Files' detail.</param>
        /// <param name="getter">File getter.</param>
        /// <param name="marker">Files marker.</param>
        /// <param name="splitPattern">splitPattern used by regex.</param>
        /// <param name="applyNewData">User handled code, return true if apply succeed.</param>
        /// <returns>Error log for each file.</returns>
        public abstract IEnumerable<string> GetFileIncreamentationContentThenApplyData(IEnumerable<Tuple<DateTime, long, string>> filesDetail, IFileGetter getter, FileMarkerManager marker, string splitPattern, Func<IEnumerable<string>, bool> applyNewData);

    }
}
