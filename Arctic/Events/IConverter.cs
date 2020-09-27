using CloudNative.CloudEvents;
using Microsoft.Azure.EventHubs;

namespace Arctic.Events
{
    public interface IConverter
    {
        CloudEvent ToCloudEvent<T>(T arcticEvent) where T : EventBase;

        T ToEvent<T>(EventData eventData) where T : EventBase;

        EventData GetEventData(CloudEvent cloudEvent);
    }
}
