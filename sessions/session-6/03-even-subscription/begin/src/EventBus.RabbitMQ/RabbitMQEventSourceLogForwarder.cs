using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using Microsoft.Extensions.Logging;

namespace eShop.EventBus;

internal sealed class RabbitMQEventSourceLogForwarder(ILoggerFactory loggerFactory) : IDisposable
{
    private static readonly Func<ErrorEventSourceEvent, Exception?, string> _formatErrorEvent =
        FormatErrorEvent;

    private static readonly Func<EventSourceEvent, Exception?, string> _formatEvent = FormatEvent;

    private readonly ILogger _logger = loggerFactory.CreateLogger("RabbitMQ.Client");
    private RabbitMQEventSourceListener? _listener;

    /// <summary>
    /// Initiates the log forwarding from the RabbitMQ event sources to a provided <see cref="ILoggerFactory"/>, call <see cref="Dispose"/> to stop forwarding.
    /// </summary>
    public void Start()
    {
        _listener ??= new RabbitMQEventSourceListener(LogEvent, EventLevel.Verbose);
    }

    private void LogEvent(EventWrittenEventArgs eventData)
    {
        var level = MapLevel(eventData.Level);
        var eventId = new EventId(eventData.EventId, eventData.EventName);

        // Special case the Error event so the Exception Details are written correctly
        if (eventData is
            {
                EventId: 3,
                EventName: "Error",
                PayloadNames: ["message", "ex"],
                Payload.Count: 2
            })
        {
            _logger.Log(level, eventId, new ErrorEventSourceEvent(eventData), null,
                _formatErrorEvent);
        }
        else
        {
            Debug.Assert(eventData is { EventId: 1, EventName: "Info" }
                or { EventId: 2, EventName: "Warn" });

            _logger.Log(level, eventId, new EventSourceEvent(eventData), null, _formatEvent);
        }
    }

    private static string FormatErrorEvent(ErrorEventSourceEvent eventSourceEvent, Exception? ex) =>
        eventSourceEvent.EventData.Payload?[0]?.ToString() ?? "<empty>";

    private static string FormatEvent(EventSourceEvent eventSourceEvent, Exception? ex) =>
        eventSourceEvent.EventData.Payload?[0]?.ToString() ?? "<empty>";

    public void Dispose() => _listener?.Dispose();

    private static LogLevel MapLevel(EventLevel level) => level switch
    {
        EventLevel.Critical => LogLevel.Critical,
        EventLevel.Error => LogLevel.Error,
        EventLevel.Informational => LogLevel.Information,
        EventLevel.Verbose => LogLevel.Debug,
        EventLevel.Warning => LogLevel.Warning,
        EventLevel.LogAlways => LogLevel.Information,
        _ => throw new ArgumentOutOfRangeException(nameof(level), level, null),
    };

    private readonly struct EventSourceEvent : IReadOnlyList<KeyValuePair<string, object?>>
    {
        public EventWrittenEventArgs EventData { get; }

        public EventSourceEvent(EventWrittenEventArgs eventData)
        {
            // only Info and Warn events are expected, which always have 'message' as the only payload
            Debug.Assert(eventData.PayloadNames?.Count == 1 &&
                eventData.PayloadNames[0] == "message");

            EventData = eventData;
        }

        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => EventData.PayloadNames?.Count ?? 0;

        public KeyValuePair<string, object?> this[int index] => new(EventData.PayloadNames![index],
            EventData.Payload![index]);
    }

    private readonly struct ErrorEventSourceEvent(EventWrittenEventArgs eventData)
        : IReadOnlyList<KeyValuePair<string, object?>>
    {
        public EventWrittenEventArgs EventData { get; } = eventData;

        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => 5;

        public KeyValuePair<string, object?> this[int index]
        {
            get
            {
                Debug.Assert(EventData.PayloadNames?.Count == 2 && EventData.Payload?.Count == 2);
                Debug.Assert(EventData.PayloadNames[0] == "message");
                Debug.Assert(EventData.PayloadNames[1] == "ex");

                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, 5);

                return index switch
                {
                    0 => new(EventData.PayloadNames[0], EventData.Payload[0]),
                    < 5 => GetExData(EventData, index),
                    _ => throw new UnreachableException()
                };

                static KeyValuePair<string, object?> GetExData(EventWrittenEventArgs eventData,
                    int index)
                {
                    Debug.Assert(index is >= 1 and <= 4);
                    Debug.Assert(eventData.Payload?.Count == 2);
                    var exData = eventData.Payload[1] as IDictionary<string, object?>;
                    Debug.Assert(exData is not null && exData.Count == 4);

                    return index switch
                    {
                        1 => new("exception.type", exData["Type"]),
                        2 => new("exception.message", exData["Message"]),
                        3 => new("exception.stacktrace", exData["StackTrace"]),
                        4 => new("exception.innerexception", exData["InnerException"]),
                        _ => throw new UnreachableException()
                    };
                }
            }
        }
    }

    /// <summary>
    /// Implementation of <see cref="EventListener"/> that listens to events produced by the RabbitMQ.Client library.
    /// </summary>
    private sealed class RabbitMQEventSourceListener : EventListener
    {
        private readonly List<EventSource> _eventSources = new();

        private readonly Action<EventWrittenEventArgs>? _log;
        private readonly EventLevel _level;

        public RabbitMQEventSourceListener(Action<EventWrittenEventArgs>? log, EventLevel level)
        {
            _log = log;
            _level = level;

            foreach (var eventSource in _eventSources)
            {
                OnEventSourceCreated(eventSource);
            }

            _eventSources.Clear();
        }

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            base.OnEventSourceCreated(eventSource);

            if (_log == null)
            {
                _eventSources.Add(eventSource);
            }

            if (eventSource.Name == "rabbitmq-dotnet-client" ||
                eventSource.Name == "rabbitmq-client")
            {
                EnableEvents(eventSource, _level);
            }
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            // Workaround https://github.com/dotnet/corefx/issues/42600
            if (eventData.EventId == -1)
            {
                return;
            }

            // There is a very tight race during the listener creation where EnableEvents was called
            // and the thread producing events not observing the `_log` field assignment
            _log?.Invoke(eventData);
        }
    }
}