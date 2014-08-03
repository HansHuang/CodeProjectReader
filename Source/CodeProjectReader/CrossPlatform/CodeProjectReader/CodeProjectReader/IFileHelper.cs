using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CodeProjectReader
{
    /// <summary>
    /// Fnterface: IFileHelper
    /// Author: Hans Huang @ Jungo Studio
    /// Create On: Augest 2nd, 2014
    /// Description: 
    /// Version: 0.1
    /// </summary> 
    public interface IFileHelper
    {
        string AppFolder { get; }

        /// <summary>
        /// Write text to file
        /// </summary>
        /// <param name="fileName">filename(contains path)</param>
        /// <param name="text">content to wroten</param>
        /// <returns></returns>
        Task SaveToFile(string fileName, string text);

        /// <summary>
        /// Write stream to file
        /// </summary>
        /// <param name="fileName">filename(contains path)</param>
        /// <param name="stream">Stream</param>
        /// <returns></returns>
        Task SaveToFile(string fileName, Stream stream);

        /// <summary>
        /// Load the content form file
        /// </summary>
        /// <param name="fileName">file name (contains path)</param>
        /// <returns></returns>
        Task<string> LoadString(string fileName);

        /// <summary>
        /// check the file existed of not
        /// </summary>
        /// <param name="fileName">filename(contains path)</param>
        /// <returns></returns>
        bool HasFile(string fileName);

        /// <summary>
        /// Get all files name under folder specified
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        List<string> GetFiles(string folderName);
    }
}
