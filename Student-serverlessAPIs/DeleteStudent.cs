using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace Student_serverlessAPIs
{
    public static class DeleteStudent
    {
        [FunctionName("DeleteStudent")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "student/{id:int}")] HttpRequest req, int id, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                using (SqlConnection connection = new SqlConnection
               (Environment.GetEnvironmentVariable("DBConnectionString", EnvironmentVariableTarget.Process)))
                {
                    string queryString = @"DELETE FROM [dbo].[Students] WHERE [Id] = @Id";
                    using (SqlCommand cmd = new SqlCommand(queryString))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Connection = connection;
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                return new OkObjectResult("Student record is deleted successfully ");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
