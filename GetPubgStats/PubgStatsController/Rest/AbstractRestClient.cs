using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;

namespace PubgStatsController.Rest
{
    public class AbstractRestClient
    {
        protected readonly RestClient client;

        public AbstractRestClient(string apiBaseUrl)
        {
            this.client = new RestClient(apiBaseUrl);
        }

        /// <summary>
        /// Creates a new request to the PUBG-web-api for the specified resource with the specified query parameters
        /// </summary>
        protected virtual RestRequest CreateRequest(RestRequestSettings settings)
        {
            RestRequest request = new RestRequest(settings.Resource, settings.HttpMethod, settings.DataFormat);
            request.AddDecompressionMethod(settings.DecompressionMethod);

            if(!(settings.Headers is null) && settings.Headers.Count > 0)
            {
                foreach(KeyValuePair<string, string> pair in settings.Headers)
                {
                    request.AddHeader(pair.Key, pair.Value);
                }
            }

            if (!(settings.QueryParameters is null) && settings.QueryParameters.Count > 0)
            {
                foreach (KeyValuePair<string, string> pair in settings.QueryParameters)
                {
                    request.AddQueryParameter(pair.Key, pair.Value);
                }
            }

            return request;
        }
    }

    public struct RestRequestSettings
    {
        public Dictionary<string, string> QueryParameters { get; set; }
        public DecompressionMethods DecompressionMethod { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public DataFormat DataFormat { get; set; }
        public Method HttpMethod { get; set; }
        public string Resource { get; set; }
    }
}
