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

        public List<Tuple<DateTime, long, string>> GetFilesDetailInfo(string destination, SearchOption searchOption, string fileExtention = null)
        {
            var files = Directory.GetFiles(destination, fileExtention != null ? "*" + fileExtention : "*", searchOption);
            var res = new List<Tuple<DateTime, long, string>>();
            foreach (var item in files)
            {
                var info = new FileInfo(item);
                res.Add(new Tuple<DateTime, long, string>(
                    info.CreationTime,
                    info.Length,
                    info.FullName));
            }
            return res;
        }
    }
}
