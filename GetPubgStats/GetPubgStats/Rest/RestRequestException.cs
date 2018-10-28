using Newtonsoft.Json;
using System.Net;
using System;

namespace GetPubgStats.Rest
{
    public class RestRequestException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }
        public RestRequestError[] Errors { get; private set; }

        public RestRequestException(HttpStatusCode statusCode, string jsonContent) : base ($"The request returned HttpStatusCode \"{statusCode}\"")
        {
            this.Errors = String.IsNullOrEmpty(jsonContent) 
                ? new RestRequestError[0]
                : JsonConvert.DeserializeObject<RestRequestErrors>(jsonContent).Errors;
            this.StatusCode = statusCode;
        }
    }


    public class RestRequestErrors
    {
        [JsonProperty("errors")]
        public RestRequestError[] Errors { get; set; }
    }

    public class RestRequestError
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
