using Common.Abstractions.Messaging;

namespace Common.Core.Messaging;

public record IntegrationEvent : Message, IIntegrationEvent;
