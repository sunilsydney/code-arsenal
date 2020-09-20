using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Shouldly;
using Arctic.Finder.Interfaces;
using Arctic.Finder;


namespace IntegrationTest
{
    public class Poc
    {
        [Fact]        
        public async Task  FetchSuggestions()
        {
            ISuggestion obj = new SuggestionFetcher();

            var res = await obj.GetSuggestions("car");

        }
    }
}
