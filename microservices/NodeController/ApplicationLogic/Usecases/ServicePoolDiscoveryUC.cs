﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using Entities.BusinessObjects.Live;
using Entities.BusinessObjects.Planned;
using Entities.BusinessObjects.States;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Usecases
{
    public class ServicePoolDiscoveryUC
    {
        public static ImmutableHashSet<string> BannedIds = ImmutableHashSet<string>.Empty;

        private readonly IPodWriteModel _podWriteModel;
        
        private readonly IPodReadModel _podReadModel;

        private readonly IEnvironmentClient _environmentClient;

        private readonly ILogger _logger;

        private static string NodeAddress => Environment.GetEnvironmentVariable("SERVICES_ADDRESS")!;

        public ServicePoolDiscoveryUC(IPodWriteModel podWriteModel, IPodReadModel podReadModel, IEnvironmentClient environmentClient, ILogger logger)
        {
            _podWriteModel = podWriteModel;
            _podReadModel = podReadModel;
            _environmentClient = environmentClient;
            _logger = logger;
        }

        public async Task DiscoverServices()
        {
            var unregisteredServices = await GetUnregisteredServices();

            foreach (var unregisteredService in unregisteredServices!)
            {
                var newPodId = Guid.NewGuid().ToString();

                var service = await _environmentClient.GetContainerInfo(unregisteredService);

                var newService = CreateService(service, newPodId);

                if(newService is null) continue;

                CreateServiceType(service);

                CreateOrUpdatePodInstance(service, newService, newPodId);
            }

            async Task<List<string>> GetUnregisteredServices()
            {
                var runningServicesIds = await _environmentClient.GetRunningServices();

                var registeredServices = _podReadModel.GetAllServices()
                    .Where(s=>s.ContainerInfo is not null && s.ServiceStatus is not LaunchedState).DistinctBy(s=>s.Id).ToDictionary(s => s.ContainerInfo!.Id);

                var filterServices = runningServicesIds?.Where(runningService => BannedIds.Contains(runningService) is false && registeredServices.ContainsKey(runningService) is false).ToList();
                
                return filterServices ?? new List<string>();
            }

            ServiceInstance? CreateService((ContainerInfo CuratedInfo, IContainerConfig RawConfig) service, string newPodId)
            {
                try
                {
                    var newService = new ServiceInstance
                    {
                        Id = service.RawConfig.Config.Config.Env.First(e => e.ToString().StartsWith("ID="))[3..],
                        ContainerInfo = service.CuratedInfo,
                        Address = NodeAddress,
                        Type = GetArtifactName(service.CuratedInfo.Labels),
                        PodId = GetPodId(service.CuratedInfo.Labels) ?? newPodId,
                        ServiceStatus = new ReadyState()
                    };

                    return newService;
                }
                catch
                {
                    // ignore because we don't control the assigned Ids, they are set in the docker compose
                    _logger.LogWarning("Invalid Service configuration found in service pool");

                    ImmutableInterlocked.Update(ref BannedIds, (set) => set.Add(service.CuratedInfo.Id));
                }

                return null;
            }

            void CreateServiceType((ContainerInfo CuratedInfo, IContainerConfig RawConfig) service)
            {
                var curatedInfoLabels = service.CuratedInfo.Labels;

                var podType = GetArtifactName(curatedInfoLabels);

                var serviceType = new ServiceType()
                {
                    ContainerConfig = service.RawConfig,
                    Type = service.CuratedInfo.Name,
                    ArtifactType = GetArtifactCategory(curatedInfoLabels),
                    IsPodSidecar = GetIsSidecar(curatedInfoLabels),
                    PodName = podType
                };

                UpdateOrCreatePodType(podType, service, serviceType);
            }

            void UpdateOrCreatePodType(string podType, (ContainerInfo CuratedInfo, IContainerConfig RawConfig) service, IServiceType serviceType)
            {
                var pod = _podReadModel.GetPodType(podType);

                if (pod is null)
                {
                    _podWriteModel.AddOrUpdatePodType(new PodType()
                    {
                        Type = podType,
                        MinimumNumberOfInstances = GetMinimumNumberOfInstances(service.CuratedInfo.Labels),
                        Gateway = GetIsSidecar(service.CuratedInfo.Labels) ? serviceType : pod?.Gateway ?? default,
                        ServiceTypes = pod?.ServiceTypes.Add(serviceType) ?? ImmutableList<IServiceType>.Empty.Add(serviceType),
                    });
                }
            }

            void CreateOrUpdatePodInstance((ContainerInfo CuratedInfo, IContainerConfig RawConfig) service, IServiceInstance newService, string newPodId)
            {
                var podId = GetPodId(service.CuratedInfo.Labels) ?? newPodId;

                var pod = _podReadModel.GetPodById(podId) ?? new PodInstance()
                {
                    ServiceInstances = ImmutableList<IServiceInstance>.Empty,
                    Type = GetPodName(service.CuratedInfo.Labels) ?? GetArtifactName(service.CuratedInfo.Labels),
                    Id = podId
                };

                pod.ServiceInstances = pod.ServiceInstances.RemoveAll(s => s.Id.Equals(newService.Id));
                pod.ServiceInstances = pod.ServiceInstances.Add(newService);

                _podWriteModel.AddOrUpdatePod(pod);
            }
        }

        private static string? GetLabelValue(ServiceLabelsEnum serviceLabels, ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            labels.TryGetValue(serviceLabels, out var label);

            return label;
        }

        private static string GetArtifactName(ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            var value = GetLabelValue(ServiceLabelsEnum.ARTIFACT_NAME, labels);

            return string.IsNullOrEmpty(value) ? throw new Exception("Artifact name not defined in compose") : value;
        }

        private static string GetArtifactCategory(ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            var value = GetLabelValue(ServiceLabelsEnum.ARTIFACT_CATEGORY, labels);

            return string.IsNullOrEmpty(value) ? nameof(ArtifactTypeEnum.Undefined) : value;
        }

        private static int GetMinimumNumberOfInstances(ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            uint.TryParse(GetLabelValue(ServiceLabelsEnum.MINIMUM_NUMBER_OF_INSTANCES, labels), out var nbInstances);
            
            return Convert.ToInt32(nbInstances);
        }

        private static string? GetPodName(ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            return GetLabelValue(ServiceLabelsEnum.POD_NAME, labels);
        }

        private static string? GetPodId(ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            return GetLabelValue(ServiceLabelsEnum.POD_ID, labels);
        }

        private static bool GetIsSidecar(ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            bool.TryParse(GetLabelValue(ServiceLabelsEnum.IS_POD_SIDECAR, labels), out var isSidecar);

            return isSidecar;
        }
    }
}
