﻿using System.Collections.Immutable;
using Entities.BusinessObjects.Live;
using Entities.DomainInterfaces.Live;

namespace ApplicationLogic.Interfaces;

public interface IEnvironmentClient
{
    public Task<ImmutableList<string>?> GetRunningServices(string[]? statuses = default);

    public Task<string?> IncreaseByOneNumberOfInstances(IContainerConfig containerConfig, string newContainerName,
        IServiceInstance serviceInstance, string podType);

    public Task RemoveContainerInstance(string containerId, bool quiet = false);

    Task RemoveVolume(string name);

    Task<(ContainerInfo CuratedInfo, IContainerConfig RawConfig)> GetContainerInfo(string containerId);

    Task GarbageCollection();

    Task SetResources(IPodInstance podInstance, long nanoCpus);
}