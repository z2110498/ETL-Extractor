using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extractor.Extract
{
    /// <summary>
    /// Factory of file getter.
    /// </summary>
    public class FileGetterFactory
    {
        /// <summary>
        /// Get file getter by getter name
        /// </summary>
        /// <param name="name">Name of the getter</param>
        /// <param name="p">Constructor parameters of target getter.</param>
        /// <returns></returns>
        public virtual IFileGetter GetFileGetterByName(string name, params string[] p)
        {
            IFileGetter res;
            switch (name)
            {
                case "Folder":
                    res = new WindowsFilesGetter();
                    break;
                case "FTPServer":
                    res = new FTPFileGetter(p[0], p[1]);
                    break;
                default:
                    throw new ArgumentException(string.Format("could not find specifed getter: {0}", name));
            }

            return res;
        }

        /// <summary>
        /// Get file getter by getter name
        /// </summary>
        /// <param name="resourceType">Getter name.</param>
        /// <param name="p">Parameters used for Constructor.</param>
        /// <returns></returns>
        public virtual IFileGetter GetFileGetterByName(ResourceType resourceType, params string[] p)
        {
            return GetFileGetterByName(resourceType.ToString(), p);
        }
    }
}
