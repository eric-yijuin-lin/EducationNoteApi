using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace isRock.Template
{
    public class LineWebHookController : isRock.LineBot.LineWebHookControllerBase
    {
        private IConfigurationRoot _configRoot;
        public LineWebHookController(IConfigurationRoot config)
        {
            _configRoot = config;
        }
        [Route("api/LineBotWebHook")]
        [HttpPost]
        public IActionResult POST()
        {
            var AdminUserId = _configRoot.GetSection("LineApi:AdminUserId").Value;

            try
            {
                //設定ChannelAccessToken
                this.ChannelAccessToken = _configRoot.GetSection("LineApi:ChannelAccessToken").Value;
                //取得Line Event
                var LineEvent = this.ReceivedMessage.events.FirstOrDefault();
                //配合Line verify 
                if (LineEvent == null) return Ok();
                if (LineEvent.replyToken == "00000000000000000000000000000000") return Ok();
                var responseMsg = "";
                //準備回覆訊息
                if (LineEvent.type.ToLower() == "message" && LineEvent.message.type == "text")
                {
                    if (LineEvent.message.text == "依玲")
                        responseMsg = "我愛依玲";
                    else if (LineEvent.message.text == "涵融")
                        responseMsg = "北七";
                    else
                        responseMsg = $"你說了: {LineEvent.message.text}";
                }
                else if (LineEvent.type.ToLower() == "message")
                    responseMsg = $"收到 event : {LineEvent.type} type: {LineEvent.message.type} ";
                else
                    responseMsg = $"收到 event : {LineEvent.type} ";
                //回覆訊息
                this.ReplyMessage(LineEvent.replyToken, responseMsg);
                //response OK
                return Ok();
            }
            catch (Exception ex)
            {
                //回覆訊息
                this.PushMessage(AdminUserId, "發生錯誤:\n" + ex.Message);
                //response OK
                return Ok();
            }
        }
    }
}