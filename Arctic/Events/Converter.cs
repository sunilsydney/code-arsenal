using CloudNative.CloudEvents;
using Microsoft.Azure.EventHubs;
using System;
using System.Collections.Generic;
using System.Text;
using Arctic.Events;
using CloudNative.CloudEvents;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net.Mime;

namespace Arctic.Events
{
    public class Converter : IConverter
    {
        public EventData GetEventData(CloudEvent cloudEvent)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Encapsulates a domain specific event in a CloudEvent.
        /// Producers MUST ensure that source + id is unique for each distinct event. 
        /// If a duplicate event is re-sent (e.g. due to a network error) it MAY have the same id. 
        /// Consumers MAY assume that Events with identical source and id are duplicates.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arcticEvent"></param>
        /// <returns></returns>
        public CloudEvent ToCloudEvent<T>(T arcticEvent) where T : EventBase
        {
            var ce = new CloudEvent(CloudEventsSpecVersion.V1_0,
                arcticEvent.Name,                
                new Uri("arctic", UriKind.Relative),
                "subject",
                Guid.NewGuid().ToString(),                   // https://github.com/cloudevents/spec/blob/master/spec.md#id
                DateTime.Now,
                new List<ICloudEventExtension>().ToArray()) // https://github.com/cloudevents/sdk-csharp#extensions
            {
                Data = arcticEvent,
                DataContentType = new ContentType(MediaTypeNames.Application.Json)                
            };
            return ce;
        }

        public T ToEvent<T>(EventData eventData) where T : EventBase
        {
            var formatter = new JsonEventFormatter();
            CloudEvent cloudEvent = formatter.DecodeStructuredEvent(eventData.Body.Array, new ICloudEventExtension[] { });

            if (cloudEvent.Data is T t)
                return t;
            else if (cloudEvent.Data is JObject jObject)
                return jObject.ToObject<T>();

            return default;
        }
    }
}
