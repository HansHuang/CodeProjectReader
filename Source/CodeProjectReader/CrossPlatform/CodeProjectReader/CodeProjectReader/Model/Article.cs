using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeProjectReader.Model
{
    /// <summary>
    /// Class: Article
    /// Author: Hans Huang @ Jungo Studio
    /// Create On: July 26th, 2014
    /// Description: The entity model of aritcle
    /// Version: 0.1
    /// </summary> 
    public class Article
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Category { get; set; }

        /// <summary>
        /// this only to mark the date of follow article
        /// when set this property, no need to other
        /// </summary>
        public DateTime Date { get; set; }

        public string DateString
        {
            get { return Date.ToString("M"); }
        }

        public string FullTitle
        {
            get { return string.Format("{0} - {1}", Title, Author); }
        }

        public Article() { }

        public Article(DateTime date,string category)
        {
            Date = date;
            Category = category;
        }
    }
}
