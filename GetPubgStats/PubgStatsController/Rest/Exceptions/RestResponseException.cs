using System.Net;
using RestSharp;
using System;

namespace PubgStatsController.Rest.Exceptions
{
    public class RestResponseException : Exception
    {
        public RestResponseException(IRestResponse restResponse, string message) : base(message)
        {
            this.RestResponse = restResponse;
        }

        public RestResponseException(IRestResponse restResponse) 
            : this(restResponse, $"Error while retrieving the resource {restResponse.Request.Resource} from the webserver")
        {}

        public IRestResponse RestResponse { get; private set; }
    }
}
