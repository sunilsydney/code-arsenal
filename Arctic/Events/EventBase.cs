using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using CloudNative.CloudEvents;
using CloudNative.CloudEvents.Extensions;

namespace Arctic.Events
{
    /// <summary>
    /// Based on CloudNative.CloudEvent 
    /// Refer https://github.com/cloudevents/sdk-csharp
    /// </summary>
    public abstract class EventBase
    {
        public string Id { get; set; }

        public abstract string Name { get; }
        public string PartitionKey { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }

        public abstract CloudEvent ToCloudNativeCloudEvent();
    }
}