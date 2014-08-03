using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodeProjectReader.Model
{
    /// <summary>
    /// Enum: ArticleType
    /// Author: Hans Huang @ Jungo Studio
    /// Create On: July 26th, 2014
    /// Description: The Type of aritcle
    /// Version: 0.1
    /// </summary> 
    [DataContract]
    public enum ArticleType
    {
        //Not support desc ?
        //[Description("Insider")]
        [EnumMemberAttribute]DailyBuilder = 1,
        [EnumMemberAttribute]WebDev,
        [EnumMemberAttribute]Mobile,
        //Insider,
        //archive of The Insiders: http://www.codeproject.com/script/Mailouts/Archive.aspx?mtpid=4
    }
}
