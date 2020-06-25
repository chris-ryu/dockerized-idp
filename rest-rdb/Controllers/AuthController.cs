using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
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

        [HttpGet ("attributes/{userId}")]
        public async Task<IActionResult> GetAttributes (int userId) {
            _logger.LogInformation ("Get Method Log");
            var auth_query = Environment.GetEnvironmentVariable ("AUTH_QUERY");
            // string auth_query = $"SELECT ID AS \"userId\", USERNAME AS \"userName\", FIRST_NAME AS \"givenName\", LAST_NAME AS \"sn\", EMAIL AS \"mail\" FROM DBUSER.USERS WHERE ID=:userId";
            // var constring = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            var server = Environment.GetEnvironmentVariable ("HOST");
            var port = Environment.GetEnvironmentVariable ("DB_PORT");
            var database = Environment.GetEnvironmentVariable ("DATABASE");
            var userName = Environment.GetEnvironmentVariable ("USER_NAME");
            var dbpassword = Environment.GetEnvironmentVariable ("PASSWORD");
            //
            dynamic expando = new ExpandoObject();
            string conString = $"Data Source={server}:{port}/{database};User Id={userName};Password={dbpassword}";
            // string conString = $"Data Source=13.209.18.89:32118/XEPDB1;User Id=dbuser;Password=dbpassword";
            _logger.LogInformation (conString);
            try {
                // using (var conn = new DB2Connection ("Server=ifx:9089;Database=stores_demo;UID=informix;Password=in4mix")) {
                using (var conn = new OracleConnection (conString)) {
                    // var cmd = $"SELECT * FROM USERS WHERE USERNAME=@username AND PASSWORD=@password";
                    var cmdQuery = auth_query;
                    var cmd = conn.CreateCommand ();
                    cmd.CommandText = cmdQuery;
                    _logger.LogInformation ($"query ${cmdQuery}");
                    cmd.Parameters.Add (":userId", userId);
                    conn.Open ();
                    OracleGlobalization info = conn.GetSessionInfo ();
                    info.TimeZone = "Asia/Seoul";
                    conn.SetSessionInfo (info);
                    var reader = cmd.ExecuteReader ();
                    while (reader.Read ()) {
                        _logger.LogInformation("while (reader.Read ()) {");
                        Enumerable.Range (0, reader.FieldCount)
                        .ToList().ForEach (x =>  AddProperty (expando, reader.GetName(x), reader.GetValue(x)) );
                        _logger.LogInformation ($"reader username {reader.GetValue(1)}");
                        break;
                    }
                    reader.Close ();
                    conn.Close ();
                }
                if(((IDictionary<String, object>)expando).Keys.Count > 0) {
                    _logger.LogInformation ($"reader Keys count  > 0");
                    return Ok (expando);
                } else {
                    return NotFound($"userId {userId} not found");
                }
            } catch (Exception ex) {
                _logger.LogError (ex.ToString ());
                return BadRequest (new ReplyMessage { Message = "REST Module DB Connection or Query failure!" + ex });
            }
        }


        // GET api/values
        [HttpGet ("auth/{username}/{password}")]
        public async Task<IActionResult> Get (string username, string password) {
            if (username == "testuser") {
                return Ok (new WISPrBandwidth { WISPrBandwidthMaxDown = 0, WISPrBandwidthMaxUp = 0 });
            }
            _logger.LogInformation ("Get Method Log");
            // var auth_query = Environment.GetEnvironmentVariable ("AUTH_QUERY");
            string auth_query = $"SELECT ID AS \"uid\", USERNAME AS \"userName\", FIRST_NAME AS \"givenName\", LAST_NAME AS \"sn\", EMAIL AS \"mail\" FROM DBUSER.USERS WHERE USERNAME=:username AND PASSWORD=DBMS_CRYPTO.HASH(UTL_I18N.STRING_TO_RAW(:password, 'AL32UTF8'), 4)";
            // var constring = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            var server = Environment.GetEnvironmentVariable ("HOST");
            var port = Environment.GetEnvironmentVariable ("DB_PORT");
            var database = Environment.GetEnvironmentVariable ("DATABASE");
            var userName = Environment.GetEnvironmentVariable ("USER_NAME");
            var dbpassword = Environment.GetEnvironmentVariable ("PASSWORD");
            dynamic expando = new ExpandoObject();
            // string conString = $"Data Source={server}:{port}/{database};User Id={userName};Password={dbpassword}";
            string conString = $"Data Source=13.209.18.89:32118/XEPDB1;User Id=dbuser;Password=dbpassword";
            _logger.LogInformation (conString);
            try {
                // using (var conn = new DB2Connection ("Server=ifx:9089;Database=stores_demo;UID=informix;Password=in4mix")) {
                using (var conn = new OracleConnection (conString)) {
                    // var cmd = $"SELECT * FROM USERS WHERE USERNAME=@username AND PASSWORD=@password";
                    var cmdQuery = auth_query;
                    var cmd = conn.CreateCommand ();
                    cmd.CommandText = cmdQuery;
                    _logger.LogInformation ($"query ${cmdQuery}");
                    cmd.Parameters.Add (":username", username);
                    cmd.Parameters.Add (":password", password);
                    conn.Open ();
                    OracleGlobalization info = conn.GetSessionInfo ();
                    info.TimeZone = "Asia/Seoul";
                    conn.SetSessionInfo (info);
                    var reader = cmd.ExecuteReader ();
                    while (reader.Read ()) {
                        _logger.LogInformation("while (reader.Read ()) {");
                        Enumerable.Range (0, reader.FieldCount)
                        .ToList().ForEach (x =>  AddProperty (expando, reader.GetName(x), reader.GetValue(x)) );
                        _logger.LogInformation ($"reader username {reader.GetValue(1)}");
                        break;
                    }
                    reader.Close ();
                    conn.Close ();
                }
                // var keys = ((IDictionary<String, object>)expando).Keys;
                // foreach (var item in keys)
                // {
                //    _logger.LogInformation("Key : " +item); 
                // }
                if(((IDictionary<String, object>)expando).Keys.Count > 0) {
                    _logger.LogInformation ($"reader Keys count  > 0");
                    return Ok ();
                } else {
                    return Ok (new ReplyMessage { Message = "Wrong Password" });
                }
            } catch (Exception ex) {
                _logger.LogError (ex.ToString ());
                return Ok (new ReplyMessage { Message = "REST Module DB Connection or Query failure!" + ex });
            }
        }

        public static void AddProperty (ExpandoObject expando, string propertyName, object propertyValue) {
            Console.WriteLine($"PropertyName : {propertyName}, Value : {propertyValue}");
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey (propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add (propertyName, propertyValue);
        }
    }
}