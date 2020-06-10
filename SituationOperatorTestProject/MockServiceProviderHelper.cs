using EquipmentLib;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SituationOperator.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZoneReader;

namespace SituationOperatorTestProject
{
    /// <summary>
    /// Helper class for tests that involve mocking ServiceProviders and Scopes. 
    /// 
    /// Tested usage is when the class to be tested is injected the IServiceProvider, and during 
    /// the test it creates a scope once, from which it resolves a service once. 
    /// This is work in progress, so don't expect it to work flawlessly in deviating situations.
    /// 
    /// Inspired by this SO post 
    /// https://stackoverflow.com/questions/44336489/moq-iserviceprovider-iservicescope
    /// </summary>
    public class MockServiceProviderHelper
    {
        /// <summary>
        /// Mocked IServiceProvider
        /// </summary>
        public Mock<IServiceProvider> ServiceProviderMock { get; set; }

        /// <summary>
        /// Mocked IServiceScope, derived from IServiceProvider.
        /// </summary>
        public Mock<IServiceScope> ServiceScopeMock { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MockServiceProviderHelper()
        {
            ServiceProviderMock = new Mock<IServiceProvider>();
            ServiceScopeMock = new Mock<IServiceScope>();

            // Setup ServiceScopeMock such that it returns the ServiceProvider
            ServiceScopeMock.Setup(x => x.ServiceProvider)
                .Returns(ServiceProviderMock.Object);

            // As IServiceProvider.CreateScope() is an extension method that can't be mocked by Moq, 
            // we have to implement/mock the internal methods and classes that are called by the extensions
            // For source code of the IServiceProvider extensions, see
            // https://github.com/aspnet/DependencyInjection/blob/master/src/DI.Abstractions/ServiceProviderServiceExtensions.cs

            // Setup a ServiceScopeFactory that returns the IServiceScopeMock
            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(ServiceScopeMock.Object);

            // Setup the ServiceProvider to return the ServiceScopeFactory
            ServiceProviderMock
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);
        }

        /// <summary>
        /// Adds Helper services to this ServiceProvider.
        /// Services added: EquipmentProvider, FileReader
        /// </summary>
        public void AddHelperServices()
        {
            // Add EquipmentProvider
            var equipmentProvider = new EquipmentProvider(
                TestHelper.GetMockLogger<EquipmentProvider>(),
                Path.Combine(TestHelper.GetTestDataFolderPath(), "EquipmentData"),
                "");
            AddService<IEquipmentProvider>(equipmentProvider);

            // Add EquipmentHelper
            var equipmentHelper = new EquipmentHelper(equipmentProvider);
            AddService<IEquipmentHelper>(equipmentHelper);

            // Bursthelper
            var burstHelper = new BurstHelper(equipmentHelper);
            AddService<IBurstHelper>(burstHelper);

            // Add FileReader
            var fileReader = new FileReader(
                TestHelper.GetMockLogger<FileReader>(),
                Path.Combine(TestHelper.GetTestDataFolderPath(), "ZoneReaderResources"));
            AddService<IZoneReader>(fileReader);
        }

        /// <summary>
        /// Adds a mocked service to the IServiceProvider as well as scopes derived from it.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="mock"></param>
        public void AddMockedService<TService>(Mock<TService> mock)
            where TService: class
        {
            // Add mocked service to the mocked IServiceProvider
            ServiceProviderMock.Setup(x => x.GetService(typeof(TService)))
                .Returns(mock.Object);
        }

        /// <summary>
        /// Adds a service to the IServiceProvider as well as scopes derived from it.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="mock"></param>
        public void AddService<TService>(TService service)
            where TService : class
        {
            // Add mocked service to the mocked IServiceProvider
            ServiceProviderMock.Setup(x => x.GetService(typeof(TService)))
                .Returns(service);
        }
    }
}
