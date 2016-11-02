using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.MP;
using Senparc.Weixin;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MvcExtension;
using WebApplication2.MessageHandler;

namespace WebApplication2.Controllers
{
    public class TestController : Controller
    {
        public readonly string Token = "SuperAdobi666hahaxxpiu";
        public readonly string AppID = "wx03a3b32bfa7efd91";
        // GET: Test
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(PostModel postModel, string echostr)
        {
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + postModel.Signature + ","
                    + CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, Token) + "。" +
                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult Post(PostModel postModel)
        {
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return Content("参数错误！");
            }

            postModel.Token = Token;
            postModel.AppId = AppID;
            //postModel.EncodingAESKey = EncodingAESKey;//根据自己后台的设置保持一致
            //postModel.AppId = AppId;//根据自己后台的设置保持一致

            var messageHandler = new CustomMessageHandler(Request.InputStream, postModel);//接收消息（第一步）

            messageHandler.Execute();//执行微信处理过程（第二步）

            return new FixWeixinBugWeixinResult(messageHandler);//返回（第三步）
        }
    }
}