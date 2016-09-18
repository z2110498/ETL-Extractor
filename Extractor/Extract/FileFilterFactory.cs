using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extractor.Extract
{
    /// <summary>
    /// Factory for file filter.
    /// </summary>
    public class FileFilterFactory
    {
        /// <summary>
        /// Get FileFilter By Name.
        /// </summary>
        /// <param name="name">Name of file filter.</param>
        /// <param name="p">Parameter used for constructor.</param>
        /// <returns></returns>
        public virtual IFileFilter GetFileFilterByName(string name, params string[] p)
        {
            IFileFilter res;
            switch (name)
            {
                case "FileUnitIncreamentation":
                    res = new FileUnitIncreamentationFilter();
                    break;
                case "FileRewriteIncreamentation":
                case "InFileIncreamentation":
                    res = new InFileIncreamentationFilter();
                    break;
                default:
                    throw new ArgumentException(string.Format("could not find specifed filter: {0}", name));
            }

            return res;
        }

        /// <summary>
        /// Get FileFilter By Name.
        /// </summary>
        /// <param name="increamentationType">Name of file filter.</param>
        /// <param name="p">Parameter used for constructor.</param>
        /// <returns></returns>
        public virtual IFileFilter GetFileFilterByName(IncreamentationType increamentationType, params string[] p)
        {
            return GetFileFilterByName(increamentationType.ToString(), p);
        }
    }
}
