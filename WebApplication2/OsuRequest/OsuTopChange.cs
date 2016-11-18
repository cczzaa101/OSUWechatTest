using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace WebApplication2.OsuRequest
{

    public class OsuUser
    {
        public string[] info;
        //public string originalString { get; set; }
        public OsuUser(string s)
        {
            //this.originalString = s;
            this.info = s.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }

    public class OsuTopChange
    {
        private readonly string APIkey = "";
        private readonly string RegofHtmltoText = "<.+?>";
        private readonly string RegofInfo = "(?=Player Name)(.*)(?=Displaying)";//(?=Displaying)";
        private List<OsuUser> getContent(int pageNumber)
        {
            string intendedURL = "http://osu.ppy.sh/p/pp/?&page="+pageNumber.ToString();// + APIkey + " & u=" + Username;
            WebClient Client = new WebClient();
            Client.Credentials = CredentialCache.DefaultCredentials;
            Byte[] PageContent = Client.DownloadData(intendedURL);
            string result = Encoding.UTF8.GetString(PageContent);
            Client.Dispose();
            Regex htmlToText = new Regex(this.RegofHtmltoText);
            result = htmlToText.Replace(result, "");
            result = result.Replace("\n", "|");
            result = result.Replace(",", "");

            result = Regex.Match(result,RegofInfo,RegexOptions.Singleline).ToString(); //避开换行符
            string[] UserList = result.Split(new char[]{ '#'}, StringSplitOptions.RemoveEmptyEntries);
            List<OsuUser> Top50 = new List<OsuUser>();
            for(int i=1;i<UserList.Length;i++)
            {
                Top50.Add(new OsuUser(UserList[i]));
            }
            return Top50; 
        }

        public string generateDebugString(int pageNumber)
        {
            List<OsuUser> UList = getContent(pageNumber);
            string s="";
            for(int i=0;i<UList.Count;i++)
            {
                for(int j=0;j<UList[i].info.Length;j++)
                {
                    s += UList[i].info[j];
                    if(j!=UList[i].info.Length-1) s+= ",";
                }
                s += "\n";
            }
            return s;
        }

    }
}
