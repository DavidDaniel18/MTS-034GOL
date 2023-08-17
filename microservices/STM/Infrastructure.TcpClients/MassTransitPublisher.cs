﻿using Application.Common.Interfaces.Policies;
using Application.EventHandlers.AntiCorruption;
using MassTransit;

namespace Infrastructure.TcpClients;

public class MassTransitPublisher : IPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IInfiniteRetryPolicy<MassTransitPublisher> _retryPolicy;

    public MassTransitPublisher(IPublishEndpoint publishEndpoint, IInfiniteRetryPolicy<MassTransitPublisher> retryPolicy)
    {
        _publishEndpoint = publishEndpoint;
        _retryPolicy = retryPolicy;
    }

    public async Task Publish<TEvent>(TEvent message) where TEvent : class
    {
        await _retryPolicy.ExecuteAsync(async () => await _publishEndpoint.Publish(message, x =>
            {
                x.SetRoutingKey("Stm.BusTrackingUpdated");
            },
            new CancellationTokenSource(TimeSpan.FromMilliseconds(50)).Token));
    }
}