using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IsolatedProcess
{
    public static class HttpRequestExtensions
    {
        private const string BadRequestErrorTypeUrl = "https://httpstatuses.com/400";
        private const string Title = "One or more validation errors occurred.";

        //Demo Extension Method
        public static bool IsGreaterThan(this int i, int value)
        {
            return i > value;
        }

        public static async Task<HttpResponseData> BadRequest(this HttpRequestData requestData, string errors)
        {
            var response = requestData.CreateResponse(HttpStatusCode.BadRequest);

            var messageBody = new
            {
                Type = BadRequestErrorTypeUrl,
                Status = HttpStatusCode.BadRequest,
                Title = Title,
                Detail = errors,
                Instance = requestData.Url.AbsoluteUri
            };

            await response.WriteAsJsonAsync(messageBody, HttpStatusCode.BadRequest);

            return response;
        }

        public static async Task<HttpResponseData> ServerError(this HttpRequestData requestData, string errors)
        {
            var response = requestData.CreateResponse(HttpStatusCode.InternalServerError);

            var messageBody = new
            {
                Type = BadRequestErrorTypeUrl,
                Status = HttpStatusCode.BadRequest,
                Title = "There was internal issues, Please contact IT Support",
                Detail = errors,
                Instance = requestData.Url.AbsoluteUri
            };

            await response.WriteAsJsonAsync(messageBody, HttpStatusCode.InternalServerError);

            return response;
        }

        public static async Task<HttpResponseData> Ok<TResult>(this HttpRequestData request, TResult result)
        {
            var response = request.CreateResponse(HttpStatusCode.OK);
            
            await response.WriteAsJsonAsync(result);

            return response;
        }

    }
}
