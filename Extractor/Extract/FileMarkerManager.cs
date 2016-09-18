using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Extractor.Extract
{
    /// <summary>
    /// Manager for file markers
    /// </summary>
    public class FileMarkerManager
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="name">Manager name which used to load and save.</param>
        private FileMarkerManager(int name)
        {
            this.Name = name;
            this.Content = new Dictionary<string, FileMarker>();
        }

        private FileMarkerManager()
        {

        }

        /// <summary>
        /// File markers
        /// </summary>
        public Dictionary<string, FileMarker> Content { get; set; }

        /// <summary>
        /// Name of the marker manager
        /// </summary>
        public int Name { get; set; }

        /// <summary>
        /// Mark a new file or reset the marker of the exist file 
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="creationTime"></param>
        /// <param name="size"></param>
        public void Set(string fullPath, DateTime creationTime, long size)
        {
            if (Content.ContainsKey(fullPath))
            {
                Content[fullPath] = new FileMarker()
                {
                    CreationTime = creationTime,
                    FullPath = fullPath,
                    Size = size,
                };
            }
            else
            {
                Content.Add(fullPath, new FileMarker()
                {
                    CreationTime = creationTime,
                    FullPath = fullPath,
                    Size = size,
                });
            }
        }

        /// <summary>
        /// Get the file size last time, return 0 if the file is not marked.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public long GetLastReadSize(string fullPath)
        {
            if (Content.ContainsKey(fullPath))
            {
                return Content[fullPath].Size;
            }

            return 0;
        }

        /// <summary>
        /// Save the file marker manager.
        /// </summary>
        /// <param name="marker"></param>
        public static void Save(FileMarkerManager marker)
        {
            if (!Directory.Exists("aaasaves"))
            {
                Directory.CreateDirectory("aaasaves");
            }

            string path = "aaasaves/" + marker.Name;
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (StreamWriter writer = new StreamWriter(
                File.Open(path, FileMode.OpenOrCreate, FileAccess.Write), System.Text.Encoding.UTF8))
            {
                writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(marker));
            }

            // add mirror file to avoid last all datas when crashing during saving.
            //
            if(!File.Exists(path + "_mirror"))
            {
                using (File.Create(path + "_mirror")) { }
            }

            using (StreamWriter writer = new StreamWriter(
                File.Open(path + "_mirror", FileMode.Truncate, FileAccess.Write), System.Text.Encoding.UTF8))
            {
                writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(marker));
            }
        }

        /// <summary>
        /// Load FileMarkerManager from local folder.
        /// </summary>
        /// <param name="markerManagerName">Name used to identify saved filename.</param>
        /// <returns></returns>
        public static FileMarkerManager Load(int markerManagerName)
        {
            string path = "aaasaves/" + markerManagerName;
            if (!File.Exists(path))
            {
                return CreateNew(markerManagerName);
            }

            using (StreamReader reader = new StreamReader(File.OpenRead(path)))
            {
                return Newtonsoft.Json.JsonConvert
                    .DeserializeObject<FileMarkerManager>(reader.ReadToEnd());
            }
        }

        /// <summary>
        /// Generate a new <see cref="FileMarkerManager"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static FileMarkerManager CreateNew(int name)
        {
            return new FileMarkerManager(name);
        }
    }

    /// <summary>
    /// File marker to mark the file's creation date, size, and full path.
    /// </summary>
    public class FileMarker
    {
        /// <summary>
        /// Creation time of the file.
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Long Size of the file.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Full path of the file.
        /// </summary>
        public string FullPath { get; set; }
    }

}
