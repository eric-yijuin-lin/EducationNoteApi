using GoogleApiLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication4.DataModels;

namespace WebApplication4.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class DailyMissionController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<DailyMissionController> _logger;
        private readonly GoogleSheetService _sheetService;

        public DailyMissionController(
            IConfiguration config,
            ILogger<DailyMissionController> logger,
            GoogleSheetService sheetService)
        {
            _config = config;
            _logger = logger;
            _sheetService = sheetService;
        }

        [HttpGet]
        public IEnumerable<DailyMissionRecord> Get()
        {
            var configSection = _config.GetSection("DailyMissionSheet");
            string credential = configSection["CredentialPath"];
            string sheetId = configSection["SheetId"];
            string tabName = configSection["TabName"];
            string range = configSection["Range"];

            var sheetContent = _sheetService.GetGoogleSheetContent(credential, sheetId, tabName, range);
            var result = DailyMissionRecord.ParseFromGoogleSheetContent(sheetContent, true);
            return result;
        }
    }
}
