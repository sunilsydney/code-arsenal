using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Arctic.Finder
{
    public static class PhraseSubmittedEventHandler
    {
        /// <summary>
        /// Uses default consumer group of Event Hub
        /// </summary>
        /// <param name="events"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("PhraseSubmittedEventHandler")]
        public static async Task Run([EventHubTrigger("phrase-submission", Connection = "EventHubsNameSpaceCS")]
            EventData[] events, ILogger log)
        {
            var exceptions = new List<Exception>();

            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);

                    // Replace these two lines with your processing logic.
                    log.LogInformation($"C# Event Hub trigger function processed a message: {messageBody}");
                    await Task.Yield();
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
    }
}
