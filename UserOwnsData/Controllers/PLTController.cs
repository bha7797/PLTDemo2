using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Mvc;
using UserOwnsData.Models;
using UserOwnsData.Services.Security;
using System.Threading.Tasks;

namespace UserOwnsData.Controllers
{
    public class PLTController : Controller
    {
        private static string getConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["TestDBEntities"].ConnectionString;
        }

        private static string getProfilePhoto()
        {
           /* var accessToken = "eyJ0eXAiOiJKV1QiLCJub25jZSI6ImoxUTBpdnJ4dDRnZnNMYnZQbTZYLTVTOXV2Z285YnNmMmZMSEhoTExXY1UiLCJhbGciOiJSUzI1NiIsIng1dCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyIsImtpZCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyJ9.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTAwMDAtYzAwMC0wMDAwMDAwMDAwMDAiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9lNGQ5OGRkMi05MTk5LTQyZTUtYmE4Yi1kYTNlNzYzZWRlMmUvIiwiaWF0IjoxNjcyNTkwODMyLCJuYmYiOjE2NzI1OTA4MzIsImV4cCI6MTY3MjU5NTc5NCwiYWNjdCI6MCwiYWNyIjoiMSIsImFpbyI6IkFWUUFxLzhUQUFBQXkwRkV0ZTlUeVJNZGtMQ2pTS21SQW9JMDN5NTJJSGlJZ3RUUzlrR0Y3OWlrVVU0VHNPTWpwdnhnemRHcmlRQWpuL2FuaDYrV0ZSK1NPeURJTnpLcG5hamhKY3c2aHYvdThicFlJZlpSQVZBPSIsImFtciI6WyJwd2QiLCJyc2EiLCJtZmEiXSwiYXBwX2Rpc3BsYXluYW1lIjoiR3JhcGggRXhwbG9yZXIiLCJhcHBpZCI6ImRlOGJjOGI1LWQ5ZjktNDhiMS1hOGFkLWI3NDhkYTcyNTA2NCIsImFwcGlkYWNyIjoiMCIsImRldmljZWlkIjoiMjU3NTUxZjEtOTBjYi00Zjc3LWIwZGItMWU4NTYyNzUzMjlhIiwiZmFtaWx5X25hbWUiOiJLdW1hciIsImdpdmVuX25hbWUiOiJNYW5vaiIsImlkdHlwIjoidXNlciIsImlwYWRkciI6IjQ5LjM3LjEzMy4yNSIsIm5hbWUiOiJNYW5vaiBLdW1hciB8IE1BUSBTb2Z0d2FyZSIsIm9pZCI6IjJiNGJhZDc2LWQxYTAtNDliOS1iNjAxLTYzNDJhYzQyMTIxMiIsInBsYXRmIjoiMyIsInB1aWQiOiIxMDAzMjAwMUNBM0QyMUREIiwicmgiOiIwLkFRY0EwbzNaNUptUjVVSzZpOW8tZGo3ZUxnTUFBQUFBQUFBQXdBQUFBQUFBQUFBSEFGMC4iLCJzY3AiOiJBY2Nlc3NSZXZpZXcuUmVhZC5BbGwgQWNjZXNzUmV2aWV3LlJlYWRXcml0ZS5BbGwgQWNjZXNzUmV2aWV3LlJlYWRXcml0ZS5NZW1iZXJzaGlwIEFkbWluaXN0cmF0aXZlVW5pdC5SZWFkLkFsbCBBZG1pbmlzdHJhdGl2ZVVuaXQuUmVhZFdyaXRlLkFsbCBBbmFseXRpY3MuUmVhZCBBUElDb25uZWN0b3JzLlJlYWQuQWxsIEFQSUNvbm5lY3RvcnMuUmVhZFdyaXRlLkFsbCBBcHBDYXRhbG9nLlJlYWQuQWxsIEFwcENhdGFsb2cuUmVhZFdyaXRlLkFsbCBBcHBDYXRhbG9nLlN1Ym1pdCBBcHBsaWNhdGlvbi5SZWFkLkFsbCBBcHBsaWNhdGlvbi5SZWFkV3JpdGUuQWxsIEFwcHJvdmFsLlJlYWQuQWxsIEFwcHJvdmFsLlJlYWRXcml0ZS5BbGwgQ2hhbm5lbE1lc3NhZ2UuUmVhZC5BbGwgRGlyZWN0b3J5LkFjY2Vzc0FzVXNlci5BbGwgRGlyZWN0b3J5LlJlYWQuQWxsIERpcmVjdG9yeS5SZWFkV3JpdGUuQWxsIEdyb3VwLlJlYWQuQWxsIEdyb3VwLlJlYWRXcml0ZS5BbGwgb3BlbmlkIHByb2ZpbGUgUmVwb3J0cy5SZWFkLkFsbCBSZXBvcnRTZXR0aW5ncy5SZWFkLkFsbCBSZXBvcnRTZXR0aW5ncy5SZWFkV3JpdGUuQWxsIFNlY3VyaXR5RXZlbnRzLlJlYWQuQWxsIFNlY3VyaXR5RXZlbnRzLlJlYWRXcml0ZS5BbGwgVXNlci5SZWFkIFVzZXIuUmVhZC5BbGwgZW1haWwiLCJzaWduaW5fc3RhdGUiOlsia21zaSJdLCJzdWIiOiJqOGdFVlUxTXBjY0d3MXVUaGhQWTVHa0NjQ3BsQ3ZXaVBLdGNPTFQyWlJZIiwidGVuYW50X3JlZ2lvbl9zY29wZSI6Ik5BIiwidGlkIjoiZTRkOThkZDItOTE5OS00MmU1LWJhOGItZGEzZTc2M2VkZTJlIiwidW5pcXVlX25hbWUiOiJtYW5vamt1QG1hcXNvZnR3YXJlLmNvbSIsInVwbiI6Im1hbm9qa3VAbWFxc29mdHdhcmUuY29tIiwidXRpIjoiNHF6QXZ6NWtnVXU5eE1JMmJTUkRBUSIsInZlciI6IjEuMCIsIndpZHMiOlsiYjc5ZmJmNGQtM2VmOS00Njg5LTgxNDMtNzZiMTk0ZTg1NTA5Il0sInhtc19zdCI6eyJzdWIiOiJFUnJ6c3laZE5PYWdwY3oxS00tVlJ4U1l6T1ByenVicnNOUnhQMFA4aXRvIn0sInhtc190Y2R0IjoxMzQyNzM5ODA4fQ.NtzqjFDLUZqlsGQS9fa1wm3ZnpiAFjJsKO3E7xz41fv_f2EzLIqnrM6qS5E-9Xa58d9QOYVnxg_grRjscsRrBV5zlOC_6Vz0asWn4Oa_xozXvKf3IM66GgYBEw2YmVUptIKL5Wxbkk8pLBMv2cNTXEbW3b1qG6tflji47vEu2leWRhW1RnqBy3PlHHGOE2zHuViz5HohEM6lLcuJcV8w6D0ZM2Xcl8_WMbJSfU32Pdx7GQkgz-LLbbDlKijX-3UKCuvvBT5GYj-xrTl-5rVcqWf_1Ek6FGjcPUM0fAtNqCYoQCNbBJb2PFwAjCTe9QxCH-Segvv3sW8g4MKJh0ka2Q";
            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me/photo/$value"))
                {
                    msg.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    using (HttpResponseMessage resp = client.SendAsync(msg).Result)
                    {
                        var res = resp.Content.ReadAsStringAsync().Result;
                        // var byteArray = res.ToArray();
                        string base64String = Convert.ToBase64String(res);
                        // JObject obj = JsonConvert.DeserializeObject<JObject>(res);
                    }
                }
            }*/
            return "random";
        }

        private static AuthDetails getUserDetails()
        {
            //getProfilePhoto();
            var userName = ClaimsPrincipal.Current.FindFirst("name").Value;

            var accessToken = (string)System.Web.HttpContext.Current.Session["AccessToken"];

            var UserEmail = (string)System.Web.HttpContext.Current.Session["UserEmail"];

            AuthDetails authDetails = new AuthDetails
            {
                UserName = userName,
                AccessToken = accessToken,
                UserEmail = UserEmail
            };

            return authDetails;
        }

        public static void storeUserDetails()
        {
            AuthDetails authDetails = getUserDetails();

            string selectQuery = "SELECT userId FROM dbo.UsersInfo where userName = @userName";
            string query;
            using (SqlConnection newConnection = new SqlConnection(getConnectionString()))
            {
                SqlCommand selectCommandSelect = new SqlCommand(selectQuery, newConnection);
                selectCommandSelect.Parameters.AddWithValue("@userName", authDetails.UserName);
                selectCommandSelect.Connection.Open();

                using (SqlDataReader reader = selectCommandSelect.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        query = @"
                                Declare @Token VARCHAR(2000)
                                SELECT @Token = @accessToken
                                Declare @Encrypt varbinary(4000)
                                SELECT @Encrypt = EncryptByPassPhrase('mysupersecretkey', @Token)
                                UPDATE dbo.UsersInfo SET accessToken = @Encrypt WHERE userName = @userName";
                    }
                    else
                    {
                        query = @"Declare @Token VARCHAR(2000)
                                SELECT @Token = @accessToken
                                Declare @Encrypt varbinary(4000)
                                SELECT @Encrypt = EncryptByPassPhrase('mysupersecretkey', @Token)
                                INSERT INTO dbo.UsersInfo (userName, accessToken) VALUES (@userName, @Encrypt)";
                    }
                }
                selectCommandSelect.Connection.Close();
                SqlCommand selectCommand = new SqlCommand(query, newConnection);
                selectCommand.Parameters.AddWithValue("@userName", authDetails.UserName);
                selectCommand.Parameters.AddWithValue("@accessToken", authDetails.AccessToken);
                selectCommand.Parameters.AddWithValue("@key", ConfigurationManager.AppSettings["dbencryptionkey"]);
                selectCommand.Connection.Open();
                try
                {
                    selectCommand.ExecuteNonQuery();
                }
                catch
                {
                    Console.WriteLine("Error occurred while Performing Insert operation.");
                }
                selectCommand.Connection.Close();
            }
        }

        private static List<PLTModel> getPLTDetails(string selectQuery, List<PLTModel> pltDetails, string workspaceName, string reportName, string userName)
        {
            using (SqlConnection newConnection = new SqlConnection(getConnectionString()))
            {
                SqlCommand selectCommandSelect = new SqlCommand(selectQuery, newConnection);
                selectCommandSelect.Parameters.AddWithValue("@userName", userName);
                if (workspaceName != null)
                {
                    selectCommandSelect.Parameters.AddWithValue("@workspacename", workspaceName);
                    if(reportName != null) selectCommandSelect.Parameters.AddWithValue("@reportname", reportName);
                }
                selectCommandSelect.Connection.Open();

                using (SqlDataReader reader = selectCommandSelect.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            PLTModel pltDetail = new PLTModel
                            {
                                Id = reader.GetInt32(0),
                                WorkspaceName = reader.GetString(1),
                                ReportName = reader.GetString(2),
                                PageName = reader.GetString(3),
                                EndTime = reader.GetString(5),
                                PLT = reader.GetDecimal(6)
                            };
                            pltDetails.Add(pltDetail);
                        }
                    }
                }
                selectCommandSelect.Connection.Close();
            }
            return pltDetails;
        }

        [Authorize]
        // GET: PLT
        public ActionResult PLTView(string workspaceName = null, string reportName = null)
        {
            List<PLTModel> pltDetails = new List<PLTModel>();

            storeUserDetails();

            try
            {
                AuthDetails userDetails = getUserDetails();
                string selectQuery = (workspaceName != null) ?
                    (
                    (reportName != null) ?
                @"SELECT L.Id
	                ,L.WorkspaceName
	                ,L.ReportName
	                ,L.PageName
                    ,COALESCE(P.EndTime, GETDATE()) as OrderDateColumn
	                ,COALESCE(P.EndTime, '')
	                ,COALESCE(P.PLT, 0)
                FROM dbo.LinksInfo L
                LEFT JOIN dbo.PLTTimeDetails P ON P.LinksId = L.Id
                LEFT JOIN dbo.UsersInfo U ON U.userId = L.UserId
                WHERE L.WorkspaceName = @workspacename
	                AND L.ReportName = @reportname
                    AND U.userName = @userName
                ORDER BY OrderDateColumn DESC" :
                @"SELECT L.Id
	                ,L.WorkspaceName
	                ,L.ReportName
	                ,L.PageName
                    ,COALESCE(P.EndTime, GETDATE()) as OrderDateColumn
	                ,COALESCE(P.EndTime, '')
	                ,COALESCE(P.PLT, 0)
                FROM dbo.LinksInfo L
                LEFT JOIN dbo.PLTTimeDetails P ON P.LinksId = L.Id
                LEFT JOIN dbo.UsersInfo U ON U.userId = L.UserId
                WHERE L.WorkspaceName = @workspacename
                    AND U.userName = @userName
                ORDER BY OrderDateColumn DESC"
                    ) :
                @"SELECT L.Id
	                ,L.WorkspaceName
	                ,L.ReportName
	                ,L.PageName
                    ,COALESCE(P.EndTime, GETDATE()) as OrderDateColumn
	                ,COALESCE(P.EndTime, '')
	                ,COALESCE(P.PLT, 0)
                FROM dbo.LinksInfo L
                LEFT JOIN dbo.PLTTimeDetails P ON P.LinksId = L.Id
                LEFT JOIN dbo.UsersInfo U ON U.userId = L.UserId
                WHERE U.userName = @userName
                ORDER BY OrderDateColumn DESC";
                pltDetails = getPLTDetails(selectQuery, pltDetails, workspaceName, reportName, userDetails.UserName);
                AuthPLTViewModel authPLTDetails = new AuthPLTViewModel()
                {
                    PLTModel = pltDetails,
                    AuthDetails = userDetails
                };
                return View(authPLTDetails);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
    }
}