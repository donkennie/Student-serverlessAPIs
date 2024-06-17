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
    public static class UpdateStudent
    {
        [FunctionName("UpdateStudent")]
        public static async Task<IActionResult> Run(
         [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "student")] HttpRequest req,
         ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Student studentData = JsonConvert.DeserializeObject<Student>(requestBody);
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("DBConnectionString", EnvironmentVariableTarget.Process)))
                {

                    string queryString = @"UPDATE [dbo].[Students] 
                            SET [Name] = @Name,[
                            Age] = @Age,[Course] 
                            = @Course,[School] = @School,
                            [Department] = @Department WHERE 
                            [Id] = @Id";

                    using (SqlCommand cmd = new SqlCommand(queryString))
                    {
                        cmd.Parameters.AddWithValue("@Name",
                       studentData.School);
                        cmd.Parameters.AddWithValue("@Age",
                       studentData.Age);
                        cmd.Parameters.AddWithValue("@Course",
                       studentData.Course);
                        cmd.Parameters.AddWithValue("@School",
                       studentData.School);
                        cmd.Parameters.AddWithValue("@Department",
                       studentData.Department);
                        cmd.Parameters.AddWithValue("@Id",
                       studentData.Id);
                        cmd.Connection = connection;
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                return new OkObjectResult(studentData);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
