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

        public string[] Get_PubgAPIKeys()
        {
            return Data.data.pubgAPI.APIKeys;
        }

        public string Get_DatabaseConnectionstring()
        {
            return Data.data.dbLayer.Connectionstring;
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
    ///  },
    ///     "DbLayer": {
    ///       "Connectionstring": "SERVER=127.0.0.1;DATABASE=xxxx;UID=xxxx;PASSWORD=xxxxxxxx;"
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

        public DbLayer dbLayer { get; set; }
        public class DbLayer
        {
            public string Connectionstring { get; set; }
        }
    }
}
