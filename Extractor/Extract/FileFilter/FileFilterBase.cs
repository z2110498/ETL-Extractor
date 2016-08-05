using System;
using System.Collections.Generic;
using System.IO;
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
        public virtual IEnumerable<string> GetFileIncreamentationContentThenApplyData(IEnumerable<Tuple<DateTime, long, string>> filesDetail, IFileGetter getter, FileMarkerManager marker, string splitPattern, Func<IEnumerable<string>, bool> applyNewData)
        {
            var errors = new List<string>();
            foreach (var detail in filesDetail)
            {
                try
                {
                    bool endOfStream = false;
                    do
                    {
                        long currentPosition;
                        var stream = getter.DownLoadFile(detail.Item3);
                        var lastReadSize = marker.GetLastReadSize(detail.Item3);
                        var lines = GetLinesBaseOnLimitedMemorySize(stream, lastReadSize, splitPattern, out currentPosition, out endOfStream);

                        if (endOfStream && NeedToRemoveTheLastOne(detail))
                        {
                            // Remove last one, reset size/position
                            if (lines.Count >= 1)
                            {
                                var offset = Encoding.UTF8.GetBytes(lines.Last()).LongLength;
                                lines.RemoveAt(lines.Count - 1);
                                currentPosition -= offset;
                            }
                        }

                        if (applyNewData(lines))
                        {
                            marker.Set(detail.Item3, detail.Item1, currentPosition);
                            FileMarkerManager.Save(marker);
                        }
                        else
                        {
                            errors.Add(string.Format("Applying error\t{0}", detail.Item3));
                            break;
                        }
                    } while (!endOfStream);
                }
                catch (Exception e)
                {
                    errors.Add(string.Format("Reading error\t{0}\t{1}", detail.Item3, e.Message));
                }

                GC.Collect();
            }

            return errors;
        }

        protected abstract bool NeedToRemoveTheLastOne(Tuple<DateTime, long, string> detail);

        protected virtual List<string> GetLinesBaseOnLimitedMemorySize(Stream stream, long startPosition, string splitPattern, out long currentPosition, out bool endOfStream)
        {
            var res = new List<string>();
            using (stream)
            {
                stream.Seek(startPosition, SeekOrigin.Begin);
                do
                {
                    res.Add(stream.ReadUnit(splitPattern, Encoding.UTF8, PatternMatchOption.MatchAsStart));

                    endOfStream = stream.EndOfStream();
                    currentPosition = stream.Position;

                    // no more than 100 MB
                    if (stream.Position - startPosition > 100000000)
                    {
                        break;
                    }
                }
                while (!endOfStream);
            }

            return res;
        }

    }
}
