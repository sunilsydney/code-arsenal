using Arctic.Events;
using CloudNative.CloudEvents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Arctic.Events
{
    public class PhraseSubmittedEvent : EventBase
    {
        public override string Name => Constants.Events.PhraseSubmittedEvent;

        [JsonProperty(Constants.Fields.Phrase)]
        public string Phrase { get; set; }

        public override CloudEvent ToCloudNativeCloudEvent()
        {
            IConverter converter = new Converter();
            return converter.ToCloudEvent<PhraseSubmittedEvent>(this);           
        }
    }
}
