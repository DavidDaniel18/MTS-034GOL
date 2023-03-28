﻿using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using ApplicationLogic.Usecases;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ApplicationLogicTests.Usecases
{
    [TestClass()]
    public class ServicePoolTest
    {
        private ServicePoolDiscoveryUC servicePoolDiscoveryUC;
      
        private Mock<IEnvironmentClient> _envMock;

        private Mock<IPodReadModel> _readModelMock;
       
        private Mock<ILogger> _logger;
        
        private Mock<IPodWriteModel> _writeModel;

        [TestInitialize]
        public void Init()
        {
            _envMock = MockProvider.GetEnvironmentMock();
            _readModelMock = MockProvider.GetReadModelMock();
            _logger = MockProvider.GetLoggerMock();
            _writeModel = MockProvider.GetWriteModelMock();

            servicePoolDiscoveryUC = new ServicePoolDiscoveryUC(
                _writeModel.Object,
                _readModelMock.Object,
                _envMock.Object,
                _logger.Object);
        }

        [TestMethod()]
        public async Task DiscoverServices()
        {
            _readModelMock.Setup(x => x.GetAllServices()).Returns(ImmutableList<IServiceInstance>.Empty);
            _readModelMock.Setup(x => x.GetAllPodTypes()).Returns(ImmutableList<IPodType>.Empty);
            _readModelMock.Setup(x => x.GetPodType(It.IsAny<string>())).Returns(() => null);

            await servicePoolDiscoveryUC.DiscoverServices();

            _writeModel.Verify(x=>x.AddOrUpdatePod(It.IsAny<IPodInstance>()), Times.Exactly(6));
            _writeModel.Verify(x=>x.AddOrUpdatePodType(It.IsAny<IPodType>()), Times.Exactly(6));
        }

        [TestMethod()]
        public async Task DiscoverServices_NoInstances()
        {
            _readModelMock.Setup(x => x.GetAllServices()).Returns(ImmutableList<IServiceInstance>.Empty);

            await servicePoolDiscoveryUC.DiscoverServices();

            _writeModel.Verify(x => x.AddOrUpdatePod(It.IsAny<IPodInstance>()), Times.Exactly(6));
            _writeModel.Verify(x => x.AddOrUpdatePodType(It.IsAny<IPodType>()), Times.Exactly(0));
        }

        [TestMethod()]
        public async Task DiscoverServices_NoNewServices()
        {
            await servicePoolDiscoveryUC.DiscoverServices();

            _writeModel.Verify(x=>x.AddOrUpdatePod(It.IsAny<IPodInstance>()), Times.Exactly(0));
        }
    }
}