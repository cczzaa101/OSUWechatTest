using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.Context;
using WebApplication2.OsuRequest;
using WebApplication2.TiebaNews;

namespace WebApplication2.MessageHandler
{
    public class CustomMessageHandler:MessageHandler<MessageContext<IRequestMessageBase, IResponseMessageBase>>
    {
        public CustomMessageHandler(Stream inputStream, PostModel postModel)
            : base(inputStream, postModel)
        {

        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var result = base.CreateResponseMessage<ResponseMessageText>();
            result.Content = "Hi~ What can I do for you";
            return result;
        }

        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            var result = base.CreateResponseMessage<ResponseMessageText>();
            if (requestMessage.Content == "GetNews")
            {
                TNews TN = new TNews();
                string[] NewsContent = TN.lastPage();
                string resultString = "";
                for(int i = NewsContent.Length-1;i >= NewsContent.Length-1 - Math.Min(NewsContent.Length - 1, 4); i--)
                {
                    resultString += NewsContent[i];
                    resultString += "\n---------------\n";
                }
                result.Content = resultString;
            }
            else
            {
                OsuLookup getInfo = new OsuLookup();
                result.Content = getInfo.getRank(requestMessage.Content);
            }
            return result;
        }


    }
}