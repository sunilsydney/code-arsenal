using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Shouldly;
using Arctic.Finder.Interfaces;
using Arctic.Finder;
using Arctic.Finder.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Http;
using System.Web;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Net;

namespace IntegrationTest
{
    public class SuggestionFetcherTest
    {
        [Fact]
        public async Task  ShouldFetchSuggestionsFromGoogle()
        {
            ISuggestion obj = new SuggestionFetcher();

            var res = await obj.GetSuggestions("cars");

            res.ShouldNotBeNull();
            res.Count.ShouldBeGreaterThan(0);
            res.First().ShouldNotBeNull();
            res.First().ShouldNotBe("");
        }
    }
}
