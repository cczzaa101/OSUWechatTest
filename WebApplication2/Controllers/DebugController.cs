using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.OsuRequest;
using WebApplication2.ViewModels;
using WebApplication2.TiebaNews;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace WebApplication2.Controllers
{
    public class DebugController : Controller
    {
        // GET: Debug
        public ActionResult Index()
        {
            DebugViewModel DVM = new DebugViewModel();
            //TNews GetData = new TNews();
            OsuTopChange OTC = new OsuTopChange();
            DVM.info = "";
            string tme = DateTime.Now.ToShortDateString();
            tme = tme.Replace("/", "-");
            StreamWriter FILE = new StreamWriter("C:\\Users\\Reimoe\\Desktop\\test\\osuRankList" + tme + ".txt");
            string temp = "";
            for (int i=1;i<=20;i++)
            {
                temp = OTC.generateDebugString(i);
                FILE.Write(temp);
            }
            FILE.Write(OTC.generateDebugString(100));
            FILE.Write(OTC.generateDebugString(200));
            FILE.Close();
            return View("Index",DVM);
        }

        public ActionResult getTNews()
        {
            TNews TestTN = new TNews();
            DebugViewModel DVM = new DebugViewModel();
            string temp = TestTN.getLastDayPageLink();
            DVM.infos = TestTN.lastPage();
            return View("getTNews", DVM);
        }

        public ActionResult getMP()
        {
            DebugViewModel DVM = new DebugViewModel();
            OsuMP MPData = new OsuMP();
            DVM.info = MPData.getMP("26364386");
            //string s = Regex.Replace(DVM.info, "\\\\u", "");
                
            //JObject JObj2 = JObject.Parse(JObj["match"].ToString());
            //JObj["games"][0]["scores"][1]["user_id"]
            return View("getMP", DVM);
        }
    }
}