using System;
using System.Collections.Generic;
using System.IO;

namespace Extractor.Extract
{
    public interface IFileGetter
    {
        /// <summary>
        /// Get files Creation timeStamp, size, and path info of the specified destination.
        /// </summary>
        /// <param name="destination">Target site or folder.</param>
        /// <param name="searchOption">Determin search files whether loop into subdirectories.</param>
        /// <param name="timeZoneOffset">zone offset base one UTC.</param>
        /// <param name="fileExtention">The file extention which need to transform.</param>
        /// <returns>List of files with Creation timeStamp, size, and path info.</returns>
        List<Tuple<DateTime, long, string>> GetFilesDetailInfo(string destination, SearchOption searchOption, int timeZoneOffset, string fileExtention = null);

        /// <summary>
        /// Opening the stream of the target file.
        /// </summary>
        /// <param name="filePath">Full path of teh taget file.</param>
        /// <returns>File stream</returns>
        Stream DownLoadFile(string filePath);
    }
}
