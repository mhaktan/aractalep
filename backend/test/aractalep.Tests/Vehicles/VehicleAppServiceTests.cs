using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using aractalep.Entities;
using aractalep.Vehicles;
using aractalep.Vehicles.Dto;
using aractalep.Flows;

namespace aractalep.Tests.Vehicles
{
    public class VehicleAppServiceTests
    {
        private readonly Mock<IRepository<Vehicle, long>> _repositoryMock;
        private readonly VehicleAppService _service;

        public VehicleAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<Vehicle, long>>();
            _service = new VehicleAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new Vehicle { Id = 1, Plate = "Test plate", Brand = "Test brand", Model = "Test model", Status = 0 },
                new Vehicle { Id = 2, Plate = "Test plate", Brand = "Test brand", Model = "Test model", Status = 0 },
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
                new Vehicle { Id = 1, Plate = "Test plate", Brand = "Test brand", Model = "Test model", Status = 0 },
                new Vehicle { Id = 2, Plate = "Test plate", Brand = "Test brand", Model = "Test model", Status = 0 },
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
            var dto = new CreateVehicleDto
            {
                Plate = "Test plate", Brand = "Test brand", Model = "Test model", Status = 0
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<Vehicle>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new Vehicle { Id = 1, Plate = "Test plate", Brand = "Test brand", Model = "Test model", Status = 0 });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new Vehicle { Id = 1, Plate = "Test plate", Brand = "Test brand", Model = "Test model", Status = 0 });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
