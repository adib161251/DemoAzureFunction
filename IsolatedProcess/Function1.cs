using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace IsolatedProcess
{
    public class Function1
    {
        private readonly ILogger _logger;

        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        [Function("Function1")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            //_logger.LogWarning("ERROR");

            var data = req.ReadFromJsonAsync<dynamic>();

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }

        [Function("Function2")]
        public HttpResponseData UserDefiend([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData request)
        {
            _logger.LogInformation("New Http Trigger function processed with a request.");

            var response = request.CreateResponse(HttpStatusCode.OK);

            response.Headers.Add("Content-Type", "application/json;charset=utf-8");

            response.WriteString("Welcome to new Azure Functions!");

            int num = 5;
            var isGreater = num.IsGreaterThan(5);

            response.WriteString("Welcome to new Azure Functions! "+ isGreater);
            return response;
        }


        [Function("FunctionWithJsonResponse")]
        public async Task<HttpResponseData> FunctionWithJsonResponse([HttpTrigger(AuthorizationLevel.Function, "post", Route = "UserData")] HttpRequestData request)
        {
            _logger.LogInformation("New Http Trigger function processed with a request");

            var requestData = await request.ReadFromJsonAsync<UserModel>();

            if(requestData == null)
            {
                return await request.BadRequest("Please provide valid request Data");
            }

            return await request.Ok(requestData);
        }



        [Function("GetAddressData")]
        public async Task<HttpResponseData> GetAddressData([HttpTrigger(AuthorizationLevel.Function,"get", Route ="GetAllAddressData")] HttpRequestData request)
        {
            _logger.LogInformation("GetAddressData request is being processed");

            //var requestData = await request.ReadFromJsonAsync<PersonModel>();
            //if(requestData == null)
            //{
            //    return await request.BadRequest("Please provide valid request Data");
            //}
            try
            {
                var db = new MongoDBCRUD("AddressBook");

                var response = db.LoadRecords<PersonModel>("Users");
                return await request.Ok(response);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return await request.ServerError(ex.Message.ToString());
            }
           
        }

        [Function("InsertAddressData")]
        public async Task<HttpResponseData> InsertAddressData([HttpTrigger(AuthorizationLevel.Function, "post", Route = "GetAllAddressData")] HttpRequestData request)
        {
            _logger.LogInformation("InsertAddressData request is being processed");

           
            try
            {
                var requestData = await request.ReadFromJsonAsync<PersonModel>();
                if (requestData == null)
                {
                    return await request.BadRequest("Please provide valid request Data");
                }

                var db = new MongoDBCRUD("AddressBook");

                requestData.InsertionDate = DateTime.Now;
                db.InsertRecord<PersonModel>("Users", requestData);
                return await request.Ok(requestData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return await request.ServerError(ex.Message.ToString());
            }

        }

        [Function("GetOnlyOneAddressData")]
        public async Task<HttpResponseData> GetOnlyOneAddressData([HttpTrigger(AuthorizationLevel.Function,"post", Route ="GetDataById")] HttpRequestData request)
        {
            _logger.LogInformation("GetOnlyOneAddressData request is being processed");
            try
            {
                var requestData = await request.ReadFromJsonAsync<PersonModel>();
                if(requestData == null)
                {
                    _logger.LogError("Please provide  valid request Data");
                    return await request.BadRequest("Please provide  valid request Data");
                }

                //var guidID = new Guid(requestData.Id.ToString());

                using(MongoDBCRUD db = new MongoDBCRUD("AddressBook"))
                {

                    var responseData = db.LoadRecordById<PersonModel>("Users", requestData.Id);

                    return await request.Ok<PersonModel>(responseData);
                }

            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return await request.ServerError(ex.Message.ToString());
            }
        }


        [Function("UpsertRecordFunction")]
        [Obsolete]
        public async Task<HttpResponseData> UpsertRecordFunction([HttpTrigger(AuthorizationLevel.Function, "post", Route ="UpsertUserInfo")] HttpRequestData request)
        {
            _logger.LogInformation("UpsertRecordFunction request is being processed");
            try
            {
                var requestData = await request.ReadFromJsonAsync<PersonModel>();
                if(requestData == null)
                {
                    _logger.LogError("Please provide  valid request Data");
                    return await request.BadRequest("Please provide  valid request Data");
                }

                using(var db = new MongoDBCRUD("AddressBook"))
                {
                    if(requestData.Id != Guid.Empty)
                    {
                        requestData.UpdatedOn = DateTime.Now.ToString();
                        var data = db.UpsertRecord<PersonModel>("Users", requestData.Id, requestData);
                        var response = new
                        {
                            Sucess = "Process was successful",
                            Data = data
                        };

                        return await request.Ok<dynamic>(response);
                    }
                    else
                    {
                        requestData.InsertionDate = DateTime.Now; 
                        db.InsertRecord<PersonModel>("Users",requestData);

                        var response = new
                        {
                            Sucess = "Process was successful",
                            Data = requestData
                        };

                        return await request.Ok<dynamic>(response);
                    }
                    
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return await request.ServerError(ex.Message.ToString());
            }
        }

        [Function("DeleteUserDataFunction")]
        public async Task<HttpResponseData> DeleteUserDataFunction([HttpTrigger(AuthorizationLevel.Function, "post", Route ="DeleteUserData")] HttpRequestData request)
        {
            _logger.LogInformation("DeleteUserDataFunction request is being processed");

            try
            {
                var requestData = await request.ReadFromJsonAsync<PersonModel>();
                
                if(requestData == null)
                {
                    _logger.LogError("Please provide  valid request Data");
                    return await request.BadRequest("Please provide  valid request Data");
                }

                using(var db = new MongoDBCRUD("AddressBook"))
                {
                    var data = db.DeleteRecord<PersonModel>("Users", requestData.Id);

                    var response = new
                    {
                        Sucess = "Process was successful",
                        Data = data
                    };

                    return await request.Ok<dynamic>(response);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return await request.ServerError(ex.Message.ToString());
            }
        }

    }
}
