using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;


namespace Configuration
{
    public class Data
    {
        static Configuration data;

        public Data(string filename)
        {

            var configuration = new ConfigurationBuilder();
            configuration.SetBasePath(System.IO.Directory.GetCurrentDirectory());
            configuration.AddJsonFile(filename);
            Data.data = configuration.Build().Get<Configuration>();
        }

        public string[] Get_bubgAPIKeys()
        {
            return Data.data.pubgAPI.APIKeys;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <example>
    /// { "PubgAPI": {
    ///    "APIKeys": [
    ///         "abc",
    ///         "abcd"
    ///    ]
    ///  }
    ///}
    /// </example>

    class Configuration
    {
        public PubgAPI pubgAPI { get; set; }
        public class PubgAPI
        {
            public string[] APIKeys { get; set; }
        }
    }
}
