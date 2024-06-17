using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Student_serverlessAPIs
{
    public static class GetStudents
    {
        [FunctionName("GetStudents")]
        public static async Task<IActionResult> Run(
         [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "student")] HttpRequest req,
         ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                List<Student> studentData = new List<Student>();
                using (SqlConnection connection = new SqlConnection
               (Environment.GetEnvironmentVariable("DBConnectionString", EnvironmentVariableTarget.Process)))
                {
                    string queryString = @"SELECT [Id]
                                 ,[Name]
                                ,[Age]
                                ,[Course]
                                ,[School]
                                ,[Department]
                                 FROM [dbo].[Students]";
                    SqlCommand cmd = new SqlCommand(queryString, connection);
                    connection.Open();
                    using (SqlDataReader oReader = cmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            Student studentInfo = new Student();
                            studentInfo.Id = (int)oReader["Id"];
                            studentInfo.Name = oReader["Name"].ToString();
                            studentInfo.Age = (int)oReader["Age"];
                            studentInfo.Course = oReader
                           ["Course"].ToString();
                            studentInfo.School = oReader
                           ["School"].ToString();
                            studentInfo.Department = oReader["Department"].ToString();
                            studentData.Add(studentInfo);
                        }
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
