using Arctic.Finder.Interfaces;
using Arctic.Finder.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Http;
using System.Web;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using System.IO.Compression;
using System.Xml;

namespace Arctic.Finder
{
    public class SuggestionFetcher : ISuggestion
    {
        private readonly string _Url = "https://www.google.com/complete/search";

        /// <summary>
        /// TODO Use strategy pattern, to implement another fetcher using
        /// API http://suggestqueries.google.com/complete/search?&output=toolbar&hl=en&q=cars&gl=AU
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns></returns>
        public async Task<List<string>> GetSuggestions(string phrase)
        {
            HttpResponseMessage httpResponse = null;

            var req = GetRequest(phrase);
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    httpResponse = await client.SendAsync(req);
                }
                return await ProcessResponse(httpResponse);                
            }
            catch(Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
            }            
            Console.Out.WriteLine(httpResponse);
            return await ProcessResponse(httpResponse);
        }

        private async Task<List<string>> ProcessResponse(HttpResponseMessage httpResponse)
        {
            var result = new List<string>();
            byte[] bytes = await httpResponse.Content.ReadAsByteArrayAsync();
            var responseString = Encoding
                .GetEncoding("iso-8859-1")
                .GetString(bytes, 0, bytes.Length);

            var xd = new XmlDocument();
            xd.LoadXml(responseString);

            var nodes = xd.DocumentElement.SelectNodes("/toplevel/CompleteSuggestion/suggestion");

            var enumerator = nodes.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var node = enumerator.Current as XmlNode;
                var val = node.Attributes[0].Value; // Same as Attributes["data"].Value;
                result.Add(val);
            }
            return result;        
        }

        /// <summary>
        /// These header values were captured from a chrome incognito window
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns></returns>
        public HttpRequestMessage GetRequest(string phrase)
        {
            // Query string params
            var builder = new UriBuilder(_Url)
            {
                Port = -1
            };
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["q"] = phrase;
            query["cp"] = $"{phrase.Length}";// Character position while typing
            query["client"] = "toolbar"; // "psy-ab";
            //query["xssi"] = "t";
            //query["gs_ri"] = "gws-wiz";
            query["hl"] = "en"; // TODO revisit while implementing multi language support
            //query["dpr"] = "1";
            //query["psi"] = "";
            //query["ei"] = "";

            // Headers

            builder.Query = query.ToString();
            string url = builder.ToString();
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            // req.Headers.CacheControl = new CacheControlHeaderValue { }; // TODO
            req.Headers.Referrer = new Uri("https://www.google.com/");
            req.Headers.Add("accept", "*/*");
            // req.Headers.Add("accept-encoding", "gzip, deflate, br"); // TODO figure out how to decompress gzip
            req.Headers.Add("accept-language", "en-US,en;q=0.9"); // TODO revisit while implementing multi language support
            //req.Headers.Add("sec-fetch-dest", "empty");
            // req.Headers.Add("cookie", ""); // TODO
            //req.Headers.Add("sec-fetch-mode", "cors");
            //req.Headers.Add("sec-fetch-site", "same-origin");
            req.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.102 Safari/537.36");

            return req;
        }
    }
}
