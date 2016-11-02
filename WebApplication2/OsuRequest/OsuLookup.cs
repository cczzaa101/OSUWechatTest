using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using WebApplication2.Models;
using System.Text.RegularExpressions;

namespace WebApplication2.OsuRequest
{
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    public class OsuLookup:OsuRequestBase
    {

        private string getProperty(string PropertyName,string Content)
        {
            Regex newReg = new Regex("(?<=.*\""+ PropertyName + "\":\")(.*?)(?=\")");
            return newReg.Match(Content).ToString();
        }

        public string getRank(string Username)
        {
            string UserContent = getContent(Username,0);
            string res = Username + '\n'+ "pp:   " + getProperty("pp_raw", UserContent) + '\n';
            res = res + "rank:   " + getProperty("pp_rank", UserContent) + '\n';
            return res;
            //return U.Username;
        }
    }
}