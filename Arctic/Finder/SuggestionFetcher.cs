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

        // TODO Use strategy pattern, to implement a fetcher using
        // this API http://suggestqueries.google.com/complete/search?&output=toolbar&hl=en&q=cars&gl=AU
        public async Task<List<string>> GetSuggestions(string phrase)
        {
            HttpResponseMessage httpResponse = null;

            var req = GetRequest(phrase);
            try
            {
                //HttpClientHandler httpClientHandler = new HttpClientHandler()
                //{
                //    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                //};

                //using (HttpClient client = new HttpClient(httpClientHandler))
                //{
                //    httpResponse = await client.SendAsync(req);
                //}

                using (HttpClient client = new HttpClient())
                {
                    httpResponse = await client.SendAsync(req);
                }

                // Crack it 
                //{
                //    var stream = await httpResponse.Content.ReadAsStreamAsync();
                //    Stream decompressedStream = new MemoryStream();
                //    using (var gs = new GZipStream(stream, CompressionMode.Decompress, false))
                //    {
                //        await gs.CopyToAsync(decompressedStream);
                //    }

                //    decompressedStream.Seek(0, SeekOrigin.Begin);

                //    StreamReader reader = new StreamReader(decompressedStream);
                //    string text = reader.ReadToEnd();

                //    //new StreamContent()
                //}
                {
                    byte[] bytes = await httpResponse.Content.ReadAsByteArrayAsync();
                    var responseString = Encoding
                        .GetEncoding("iso-8859-1")
                        .GetString(bytes, 0, bytes.Length);

                    var xd = new XmlDocument();
                    xd.LoadXml(responseString);
                    var node_titles = xd.DocumentElement.SelectNodes("/toplevel/CompleteSuggestion/suggestion");

                    var res1 = xd.DocumentElement.SelectNodes("/toplevel/CompleteSuggestion/suggestion")[0].Attributes[0].Value;
                    var res2 = xd.DocumentElement.SelectNodes("/toplevel/CompleteSuggestion/suggestion")[0].Attributes["data"].Value;
                }
                {
                    string res = await httpResponse.Content.ReadAsStringAsync();
                    byte[] bytes = await httpResponse.Content.ReadAsByteArrayAsync();
                    string s1 = Encoding.UTF8.GetString(bytes);
                    string s2 = Encoding.ASCII.GetString(bytes);
                    string s3 = Encoding.Default.GetString(bytes);
                    string s4 = Encoding.Unicode.GetString(bytes);
                    string s5 = Encoding.UTF32.GetString(bytes);
                    string s6 = Encoding.BigEndianUnicode.GetString(bytes);
                    string s7 = Encoding.UTF7.GetString(bytes);

                    System.Text.Encoding iso_8859_1 = System.Text.Encoding.GetEncoding("iso-8859-1");
                    var bytesUTF8 = System.Text.Encoding.Convert(iso_8859_1, Encoding.UTF8, bytes);
                    string sUTF8 = Encoding.UTF8.GetString(bytesUTF8);
                }

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
            string response = await httpResponse.Content.ReadAsStringAsync();

            File.WriteAllText(@"C:\temp\f1.txt", response + "\r\n");

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new Exception($"{httpResponse.ReasonPhrase} {response}");
            }

            // Experiment 1
            var bytes = await httpResponse.Content.ReadAsByteArrayAsync();
            File.WriteAllBytes(@"C:\temp\f2.txt", bytes);

            var rString = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            File.WriteAllText(@"C:\temp\f3.txt", rString + "\r\n");

            if (rString[0] == '\uFEFF')
            {
                rString = rString.Substring(1);
            }

            //// Experiment 2
            //var enumerator = httpResponse.Headers.GetEnumerator();
            //while(enumerator.MoveNext())
            //{
            //    var kvp = enumerator.Current;
            //    string key = kvp.Key;
            //    string val = kvp.Value?.First() ?? string.Empty;
            //    // Find Content-Type, Content-Encoding, Content-Disposition
            //}

            // Experiment 3           

            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(GoogleResponse));

                int pos = response.IndexOf("<toplevel>");
                response = response.Substring(pos);

                var bytesUniCode = Encoding.UTF8.GetBytes(response);

                using (Stream stream = new MemoryStream(bytesUniCode))
                {
                    var obj = (GoogleResponse)xmlSerializer.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                throw;
            }

            return new List<string>();            
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
