using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

using webapi.Model;

namespace webapi.Controllers {
    [ApiController]
    public class AuthController : ControllerBase {
        ILogger<AuthController> _logger;
        public AuthController (ILogger<AuthController> logger) {
            _logger = logger;
        }

        [HttpGet ("check/{username}/{password}")]
        public async Task<IActionResult> GetCheck (string username, string password) {
            return Ok ();
        }

        [HttpGet ("accounting/{username}")]
        public async Task<IActionResult> GetAccounting (string username) {
            return Ok ();
        }

        // GET api/values
        [HttpGet ("auth/{username}/{password}")]
        public async Task<IActionResult> Get (string username, string password) {
            if(username=="testuser"){
                return Ok(new WISPrBandwidth { WISPrBandwidthMaxDown = 0, WISPrBandwidthMaxUp = 0 });
            }
            _logger.LogInformation ("Get Method Log");
            var resultList = new ArrayList ();
            // var auth_query = Environment.GetEnvironmentVariable ("AUTH_QUERY");
            string auth_query = $"SELECT * FROM DBUSER.USERS WHERE USERNAME=:username AND PASSWORD=DBMS_CRYPTO.HASH(UTL_I18N.STRING_TO_RAW(:password, 'AL32UTF8'), 4)";
            // var constring = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            var server = Environment.GetEnvironmentVariable ("HOST");
            var port = Environment.GetEnvironmentVariable ("DB_PORT");
            var database = Environment.GetEnvironmentVariable ("DATABASE");
            var userName = Environment.GetEnvironmentVariable ("USER_NAME");
            var dbpassword = Environment.GetEnvironmentVariable ("PASSWORD");

            // string conString = $"Data Source={server}:{port}/{database};User Id={userName};Password={dbpassword}";
            string conString = $"Data Source=13.209.18.89:32118/XEPDB1;User Id=dbuser;Password=dbpassword";
            _logger.LogInformation(conString);
            try {

                // using (var conn = new DB2Connection ("Server=ifx:9089;Database=stores_demo;UID=informix;Password=in4mix")) {
                using (var conn = new OracleConnection (conString)) {
                    // var cmd = $"SELECT * FROM USERS WHERE USERNAME=@username AND PASSWORD=@password";
                    var cmdQuery = auth_query;
                    var cmd = conn.CreateCommand ();
                    cmd.CommandText = cmdQuery;
                    _logger.LogInformation($"query ${cmdQuery}");
                    cmd.Parameters.Add (":username", username);
                    cmd.Parameters.Add (":password", password); 
                    conn.Open ();

                    OracleGlobalization info = conn.GetSessionInfo();
                    info.TimeZone = "Asia/Seoul";
                    conn.SetSessionInfo(info);
                    var results = cmd.ExecuteReader ();
                    while (results.Read ()) {
                        var values = new object[results.FieldCount];
                        results.GetValues (values);
                        resultList.Add (values);
                    }
                    results.Close ();
                    conn.Close ();
                }
                _logger.LogInformation($"resultList Count {resultList.Count}");
                if (resultList.Count > 0) {
                    return Ok (new WISPrBandwidth { WISPrBandwidthMaxDown = 0, WISPrBandwidthMaxUp = 0 });
                } else {
                    return Ok (new ReplyMessage { Message = "Wrong Password" });
                }
            } catch (Exception ex) {
                _logger.LogError(ex.ToString());
                return Ok (new ReplyMessage { Message = "REST Module DB Connection or Query failure!" + ex });
            }

        }

        // GET api/values/5
    }
}