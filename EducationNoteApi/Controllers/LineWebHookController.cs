using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleApiLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace isRock.Template
{
    public class LineWebHookController : isRock.LineBot.LineWebHookControllerBase
    {
        private IConfiguration _configRoot;
        private GoogleSheetService _sheetService;

        public LineWebHookController(IConfiguration config, GoogleSheetService sheetService)
        {
            _configRoot = config;
            _sheetService = sheetService;
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

        [Route("api/LineBotDemo")]
        [HttpPost]
        public IActionResult Demo() // https://goattl.com/EducationNote/api/LineBotWebHook
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
                    if (string.IsNullOrEmpty(LineEvent.message.text))
                        responseMsg = "請輸入文字喔";
                    else
                    {
                        var configSection = _configRoot.GetSection("DailyMissionSheet");
                        string credential = configSection["CredentialPath"];
                        string sheetId = configSection["SheetId"];
                        string tabName = configSection["TabName"];
                        string range = configSection["Range"];

                        var sheetContent = _sheetService.GetGoogleSheetContent(credential, sheetId, "DEMO", "A:B");
                        var row = sheetContent.FirstOrDefault(x => x[0].ToString() == LineEvent.message.text);
                        if (row == null)
                        {
                            responseMsg = $"搜尋不到 {LineEvent.message.text} 對應的資料";
                        }
                        else
                        {
                            var data = row[1];
                            if (data == null || string.IsNullOrEmpty(data.ToString()))
                            {
                                responseMsg = $"搜尋不到 {LineEvent.message.text} 對應的資料";
                            }
                            else
                            {
                                responseMsg = $"{data}";
                            }
                        }
                    }
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