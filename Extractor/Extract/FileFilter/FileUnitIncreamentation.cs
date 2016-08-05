using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Extractor.Extract
{
    /// <summary>
    /// Filter strategy of FileUnitIncreatation type
    /// </summary>
    public class FileUnitIncreamentationFilter : FileFilterBase
    {
        /// <summary>
        /// Filter our the Increamentation Files details by comparing marker and start time
        /// </summary>
        /// <param name="filesDetail">Files' detail.</param>
        /// <param name="StartTime">date time threshold used to idenfy whether a file is valid and extractable.</param>
        /// <param name="marker">Files marker.</param>
        /// <returns>Extractable files details</returns>
        public override IEnumerable<Tuple<DateTime, long, string>> FilterIncreamentationFileDetails(IEnumerable<Tuple<DateTime, long, string>> filesDetail, DateTime StartTime, FileMarkerManager marker)
        {
            var res = new List<Tuple<DateTime, long, string>>();
            foreach (var detail in filesDetail)
            {
                if (!marker.Content.ContainsKey(detail.Item3) && detail.Item1 > StartTime)
                {
                    res.Add(detail);
                }
            }
            return res;
        }

        protected override bool NeedToRemoveTheLastOne(Tuple<DateTime, long, string> detail)
        {
            return false ;
        }
    }
}
