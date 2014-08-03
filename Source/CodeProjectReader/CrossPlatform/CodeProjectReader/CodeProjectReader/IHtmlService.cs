using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeProjectReader.Model;
using Xamarin.Forms;

namespace CodeProjectReader
{
    /// <summary>
    /// Interface: IHtmlService
    /// Author: Hans Huang @ Jungo Studio
    /// Create On: August 2nd, 2014
    /// Description: Requirment for generate/load local html file logic
    /// Version: 0.1
    /// </summary> 
    public interface IHtmlService
    {
        string BaseFolder { get; }

        /// <summary>
        /// Inittal templte html file, css and images to app loacl storge
        /// </summary>
        /// <returns>Running task</returns>
        Task InittalHtml();

        /// <summary>
        /// Download html data(images,htm file) for each article
        /// </summary>
        /// <param name="articles"></param>
        void DownloadHtmlData(List<Article> articles);

    }
}
