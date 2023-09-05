﻿using Application.EventHandlers;

// MassTransit URN type resolutions, namespaces must be equal between projects for a shared type 
// ReSharper disable once CheckNamespace
namespace Contracts;

public class StaticGtfsDataUpdated : Event
{
    public StaticGtfsDataUpdated(Guid id, DateTime created) : base(id, created)
    {
    }
}