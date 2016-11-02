using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Menu;

namespace WebApplication2.Controllers
{
    public class MenuController : Controller
    {
        // GET: Menu
        private readonly string AppId = "wx03a3b32bfa7efd91";
        private readonly string AppSecret = "cf1176b76b294c6a3bbb5eb56342fcaa";
        [HttpPost]
        public ActionResult Index()
        {
            var token = CommonApi.GetToken(AppId, AppSecret);
            ButtonGroup bg = new ButtonGroup();

            //单击
            bg.button.Add(new SingleClickButton()
            {
                name = "单击测试",
                key = "OneClick",
                //type = ButtonType.click.ToString(),//默认已经设为此类型，这里只作为演示
            });

            var result = Json( CommonApi.CreateMenu(token.ToString(), bg));
            return result;
        }
    }
}