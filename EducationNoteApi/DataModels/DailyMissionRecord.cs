using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication4.DataModels
{
    public class DailyMissionRecord
    {
        private static Dictionary<string, string> _missionTypes = new Dictionary<string, string>()
        {
            {"必要", "required"},
            {"心靈", "mental"},
            {"技能", "skill"},
            {"運動", "exercise"},
            {"社交", "social"},
            {"娛樂", "entertainment"},
        };

        public static string GetTypeByChinese(string chineseType)
        {
            return _missionTypes[chineseType];
        }

        public static List<DailyMissionRecord> ParseFromGoogleSheetContent(IList<IList<object>> content, bool skipHeader)
        {
            var result = new List<DailyMissionRecord>();
            int startIndex = skipHeader ? 1 : 0;
            for (int i = startIndex; i < content.Count; i++)
            {
                var tempRow = new DailyMissionRecord()
                {
                    FinishedDate = DateTime.Parse(content[i][0].ToString()),
                    MissionType = DailyMissionRecord.GetTypeByChinese(content[i][1].ToString()),
                    Subject = content[i][2].ToString(),
                    Point = Int32.Parse(content[i][3].ToString()),
                    Description = content[i].Count > 4 ? content[i][4].ToString() : ""
                };
                result.Add(tempRow);
            }

            return result;
        }

        public DateTime FinishedDate { get; set; }
        public string MissionType { get; set; }
        public string Subject { get; set; }
        public int Point { get; set; }
        public string Description { get; set; }
    }
}
