using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace WebApplication2.OsuRequest
{
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    //using System.
    public enum Mods
    {
        None = 0,
        NoFail = 1,
        Easy = 2,
        //NoVideo      = 4,
        Hidden = 8,
        HardRock = 16,
        SuddenDeath = 32,
        DoubleTime = 64,
        Relax = 128,
        HalfTime = 256,
        Nightcore = 512, // Only set along with DoubleTime. i.e: NC only gives 576
        Flashlight = 1024,
        Autoplay = 2048,
        SpunOut = 4096,
        Relax2 = 8192,  // Autopilot?
        Perfect = 16384,
        Key4 = 32768,
        Key5 = 65536,
        Key6 = 131072,
        Key7 = 262144,
        Key8 = 524288,
        keyMod = Key4 | Key5 | Key6 | Key7 | Key8,
        FadeIn = 1048576,
        Random = 2097152,
        LastMod = 4194304,
        FreeModAllowed = NoFail | Easy | Hidden | HardRock | SuddenDeath | Flashlight | FadeIn | Relax | Relax2 | SpunOut | keyMod,
        Key9 = 16777216,
        Key10 = 33554432,
        Key1 = 67108864,
        Key3 = 134217728,
        Key2 = 268435456
    }

    public class OsuRequestBase
    {
        private readonly string APIkey = "f06cd7234c6979b892a4ea9a6226f128183231d7";
        private readonly string[] APIType = { "get_user?u=","get_match?mp=","get_beatmaps?b="};
        public string getContent(string requestData,int AType)
        {
            string intendedURL = "http://osu.ppy.sh/api/" + APIType[AType] + requestData + "&k=" + APIkey;
            WebClient Client = new WebClient();
            Client.Credentials = CredentialCache.DefaultCredentials;
            Byte[] PageContent = Client.DownloadData(intendedURL);
            string result = Encoding.UTF8.GetString(PageContent);
            return result;
        }
    }

    public class OsuMapInfo:OsuRequestBase
    {
        private JObject Content;// { get; set; }
        public OsuMapInfo(string MapID)
        {
            string temp = getContent(MapID, 2);
            temp = temp.Substring(1, temp.Length - 2);
            Content = JObject.Parse(temp);
        }
        public double getDiff()
        {
            return Convert.ToDouble( this.Content["difficultyrating"].ToString());
        }
        public double getLength()
        {
            return Convert.ToDouble(this.Content["total_length"].ToString()) / 60;
        }
    }

    public class UserPerformanceInMatch
    {
        public int maxScore = 1000000; //?????无法解决
        public string UserName { get; set; }
        public int Scores;
        public double PP;
        public Mods OverallMod;
        //public double mapLength;
        //public double elapsedDate;
        //public double starRating;
        public double baseFactor;
        public double othersPerformance = 0;
        public JObject RawContent;
        public UserPerformanceInMatch(Mods M, string SlotInfo,double mapLength,double elapsedDate,double starRating)
        {
            this.OverallMod = M;
            RawContent = JObject.Parse(SlotInfo);
            UserName = RawContent["user_id"].ToString();
            this.Scores = Convert.ToInt32(RawContent["score"].ToString());
            //this.PP = new OsuUserInfo(UserName).getPP();

            baseFactor = Math.Pow(0.94, elapsedDate) * Math.Pow(1.06, mapLength) * Scores * Math.Pow(1.19, starRating) / maxScore;

            switch (M)
            {
                case Mods.DoubleTime:
                    baseFactor *= 1.5;
                    break;
                case Mods.HardRock:
                    baseFactor *= 1.12;
                    break;
                case Mods.Hidden:
                    baseFactor *= 1.06;
                    break;
                default:
                    break;

            }
        }

        public void addOpponents(double pp,int score)
        {
            this.othersPerformance += (pp * score * baseFactor) / maxScore /124.50;
        }
    }

    public class OsuUserInfo
    {
        public string Username { get; set; }
        public double PP = -1;
        JObject UserData { get; set; }
        public OsuUserInfo(string UserID)
        {
            OsuRequestBase temp = new OsuRequestBase();
            string Content = temp.getContent(UserID, 0);
            this.UserData = JObject.Parse(Content.Substring(1, Content.Length - 2));
            this.PP = getPP();
        }

        private double getPP()  
        {
            return Convert.ToDouble( this.UserData["pp_raw"].ToString() );
        }
    }

    public class MPer:OsuUserInfo
    {
        double MPpoints=0;
        public List<UserPerformanceInMatch> MList = new List<UserPerformanceInMatch>();
        public MPer(string s) : base(s) { }
        public void addMatch(UserPerformanceInMatch UPIM)
        {
            this.MList.Add(UPIM);
            MPpoints = MPpoints * 0.95;
            MPpoints += UPIM.baseFactor*UPIM.othersPerformance;
        }
    }

    public class OsuMP:OsuRequestBase
    {
        Dictionary<string, MPer> MPerDictionary = new Dictionary<string, MPer>();
        public string getMP(string MPId)
        {
            string res = getContent(MPId, 1);
            JObject JObj = JObject.Parse(res);
            
            for(int i=0;i<JObj["games"].Count();i++)
            {
                OsuMapInfo mapInThisGame = new OsuMapInfo(JObj["games"][i]["beatmap_id"].ToString());
                double Length = mapInThisGame.getLength();
                double Diff = mapInThisGame.getDiff();
                List<UserPerformanceInMatch> PList = new List<UserPerformanceInMatch>();

                Mods OverallMod = (Mods)Enum.Parse(typeof(Mods), JObj["games"][i]["mods"].ToString(), true);
                for (int j = 0;j<JObj["games"][i]["scores"].Count();j++)
                {
                    PList.Add(
                        new UserPerformanceInMatch(OverallMod,
                        JObj["games"][i]["scores"][j].ToString(),
                        Length,
                        0,
                        Diff));
                }

                PList = PList.OrderBy(s => s.Scores).ToList();

                for (int j = 0; j < PList.Count(); j++)
                {
                    if (!MPerDictionary.ContainsKey(PList[j].UserName))
                        MPerDictionary[PList[j].UserName] = new MPer(PList[j].UserName);
                }

                for (int j=0;j<PList.Count();j++)
                {
                    for (int k = j + 1; k < PList.Count(); k++)
                        PList[k].addOpponents(MPerDictionary[ PList[j].UserName ].PP, PList[j].Scores);
                    MPerDictionary[PList[j].UserName].addMatch(PList[j]);
                }
            }

            return res;
        }
    }
}