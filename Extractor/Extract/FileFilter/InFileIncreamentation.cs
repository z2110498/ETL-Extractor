using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Extractor.Extract
{
    /// <summary>
    /// Filter strategy of InFileIncreatation type
    /// </summary>
    public class InFileIncreamentationFilter : FileFilterBase
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
                if (detail.Item1 < StartTime)
                {
                    continue;
                }

                if (!marker.Content.ContainsKey(detail.Item3))
                {
                    res.Add(detail);
                }
                // in file increamentation filter
                //
                else if (detail.Item2 > marker.Content[detail.Item3].Size)
                {
                    res.Add(detail);
                }
            }
            return res;
        }

        protected override bool NeedToRemoveTheLastOne(Tuple<DateTime, long, string> detail)
        {
            // last one need to remove in 12 hours
            return detail.Item1 - DateTime.UtcNow < new TimeSpan(12, 0, 0);
        }
    }

}
