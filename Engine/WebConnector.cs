using System;

namespace Engine
{
    public class WebConnector
    {
        private string baseUrl;

        //https://api.covid19api.com/total/country/poland?from=2021-03-26T01:00:00Z&to=2021-03-27T00:00:00Z

        public WebConnector(string bUrl)
        {
            baseUrl = bUrl;
        }
    }
}
