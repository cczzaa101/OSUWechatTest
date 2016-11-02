using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

namespace WebApplication2.TiebaNews
{
    public class TiebaLink
    {
        private string TiebaURL { get; set; }
        public byte[] content;
        public TiebaLink(string postNumber)
        {
            this.TiebaURL = "http://tieba.baidu.com" + postNumber;
            using (WebClient Client = new WebClient())
            { 
                Client.Credentials = CredentialCache.DefaultCredentials;
                this.content = Client.DownloadData(this.TiebaURL);
            }
        }
    }

    public class TNews
    {
        //<a href="/p/1413928634?pn=274">尾页</a>
        //content j_d_post_content ">
        //</div><br> </cc>
        private readonly string YiwenluNumber = "/p/1413928634";
        private readonly string RegoflastPageLink = "(?<=.*href\\=\")(.*?)(?=\"\\>尾页.*)";
        private readonly string LastPageNum = "(?<=pn\\=)(.*)";
        private readonly string RegoflastPageContent = "(?<=j_d_post_content \"\\> )(.*?)(?=\\<\\/div\\>.*)";
        private readonly string RegofHtmltoText = "<.+?>";
        //;(?=\\<\\/ div\\>\\< br\\> \\<\\/ cc\\> \\< br\\>)

        public string getLastDayPageLink()
        {
            TiebaLink TL = new TiebaLink(YiwenluNumber);
            string result = Encoding.UTF8.GetString(TL.content);
            string lastPageNumber = Regex.Match(getLastPageLink(), LastPageNum).ToString();
            return result;
        }
        private string getLastPageLink()
        {
            TiebaLink TL = new TiebaLink(YiwenluNumber);
            string result = Encoding.UTF8.GetString(TL.content);
            Regex test = new Regex(RegoflastPageLink);
            result = test.Match(result).ToString();
            return result;
        }

        public string[] lastPage()
        {
            TiebaLink TL = new TiebaLink(getLastPageLink());
            string res = Encoding.UTF8.GetString(TL.content);
            Regex getPost = new Regex(RegoflastPageContent);
            MatchCollection newRes = getPost.Matches(res) ;
            string[] News = new string[newRes.Count];
            Regex textualizePost = new Regex(RegofHtmltoText);

            for(int i=0;i<newRes.Count;i++)
            {
                News[i] = newRes[i].ToString();
                News[i] = textualizePost.Replace(News[i], " ");
            }
            //result = test.Match(result).ToString();
            return News;
        }


    }
}