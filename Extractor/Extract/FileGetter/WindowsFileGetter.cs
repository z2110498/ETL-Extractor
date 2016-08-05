using System;
using System.Collections.Generic;
using System.IO;

namespace Extractor.Extract
{
    /// <summary>
    /// Windows platform based files getter
    /// </summary>
    public class WindowsFilesGetter : IFileGetter
    {
        public Stream DownLoadFile(string filePath)
        {
            return File.OpenRead(filePath);
        }

        /// <summary>
        /// Get files Creation timeStamp, size, and path info of the specified destination.
        /// </summary>
        /// <param name="destination">Target site or folder.</param>
        /// <param name="searchOption">Determin search files whether loop into subdirectories.</param>
        /// <param name="fileExtention">The file extention which need to transform.</param>
        /// <param name="timeZoneOffset">zone offset base one UTC.</param>
        /// <returns>List of files with Creation timeStamp, size, and path info.</returns>
        public List<Tuple<DateTime, long, string>> GetFilesDetailInfo(string destination, SearchOption searchOption, int timeZoneOffset, string fileExtention = null)
        {
            var files = Directory.GetFiles(destination, fileExtention != null ? "*" + fileExtention : "*", searchOption);
            var res = new List<Tuple<DateTime, long, string>>();
            foreach (var item in files)
            {
                var info = new FileInfo(item);
                res.Add(new Tuple<DateTime, long, string>(
                    info.CreationTime.AddHours(timeZoneOffset),
                    info.Length,
                    info.FullName));
            }
            return res;
        }
    }
}
