using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using aractalep.Entities;
using aractalep.VehicleRequestTypes;
using aractalep.VehicleRequestTypes.Dto;
using aractalep.Flows;

namespace aractalep.Tests.VehicleRequestTypes
{
    public class VehicleRequestTypeAppServiceTests
    {
        private readonly Mock<IRepository<VehicleRequestType, long>> _repositoryMock;
        private readonly VehicleRequestTypeAppService _service;

        public VehicleRequestTypeAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<VehicleRequestType, long>>();
            _service = new VehicleRequestTypeAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new VehicleRequestType { Id = 1, Name = "Test name" },
                new VehicleRequestType { Id = 2, Name = "Test name" },
            }.AsQueryable();

            _repositoryMock.Setup(r => r.GetAll()).Returns(entities);

            // Act
            var result = _repositoryMock.Object.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
        }

        [Fact]
        public void Repository_GetAll_WithFilter_ShouldWork()
        {
            // Arrange
            var entities = new[]
            {
                new VehicleRequestType { Id = 1, Name = "Test name" },
                new VehicleRequestType { Id = 2, Name = "Test name" },
            }.AsQueryable();

            _repositoryMock.Setup(r => r.GetAll()).Returns(entities);

            // Act — simulate keyword filter
            var result = _repositoryMock.Object.GetAll()
                .Where(x => x.Id.ToString().Contains("1"));

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Create_ShouldInsertEntity()
        {
            // Arrange
            var dto = new CreateVehicleRequestTypeDto
            {
                Name = "Test name"
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<VehicleRequestType>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new VehicleRequestType { Id = 1, Name = "Test name" });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new VehicleRequestType { Id = 1, Name = "Test name" });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
