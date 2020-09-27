using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arctic.Events;
using Arctic.Finder.Interfaces;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Arctic.Finder
{
    public class PhraseSubmittedEventHandler
    {
        ISuggestion _suggestionFetcher;
        public PhraseSubmittedEventHandler(ISuggestion suggestion)
        {
            _suggestionFetcher = suggestion ?? throw new ArgumentNullException(nameof(suggestion));
        }

        /// <summary>
        /// Uses default consumer group of Event Hub
        /// </summary>
        /// <param name="events"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("PhraseSubmittedEventHandler")]
        public async Task Run([EventHubTrigger("phrase-submission", Connection = "EventHubsNameSpaceCS")]
            EventData[] events, ILogger log)
        {
            var exceptions = new List<Exception>();

            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    log.LogInformation($"C# Event Hub trigger function processed a message: {messageBody}");

                    await Process(eventData);
                }
                catch (Exception e)
                {
                    // Need to keep processing the rest of the batch 
                    // capture this exception and continue.
                    exceptions.Add(e);
                }
            }

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }

        public async Task Process(EventData eventData)
        {
            IConverter converter = new Converter();

            PhraseSubmittedEvent phraseSubmittedEvent = converter.ToEvent<PhraseSubmittedEvent>(eventData);
            var phrase = phraseSubmittedEvent.Phrase;

            var suggestions = await _suggestionFetcher.GetSuggestions(phrase);
        }


    }
}
