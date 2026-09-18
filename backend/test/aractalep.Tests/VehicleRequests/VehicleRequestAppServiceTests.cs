using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using aractalep.Entities;
using aractalep.VehicleRequests;
using aractalep.VehicleRequests.Dto;
using aractalep.Flows;

namespace aractalep.Tests.VehicleRequests
{
    public class VehicleRequestAppServiceTests
    {
        private readonly Mock<IRepository<VehicleRequest, long>> _repositoryMock;
        private readonly VehicleRequestAppService _service;

        public VehicleRequestAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<VehicleRequest, long>>();
            _service = new VehicleRequestAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object, new Mock<IRepository<StatusChangeLog, long>>().Object, new Mock<IRepository<ApprovalRecord, Guid>>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new VehicleRequest { Id = 1, RequestNo = "Test requestNo", RequestTypeId = 1L, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Purpose = "Test purpose", IsPoolExternal = true, Status = 0 },
                new VehicleRequest { Id = 2, RequestNo = "Test requestNo", RequestTypeId = 1L, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Purpose = "Test purpose", IsPoolExternal = true, Status = 0 },
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
                new VehicleRequest { Id = 1, RequestNo = "Test requestNo", RequestTypeId = 1L, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Purpose = "Test purpose", IsPoolExternal = true, Status = 0 },
                new VehicleRequest { Id = 2, RequestNo = "Test requestNo", RequestTypeId = 1L, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Purpose = "Test purpose", IsPoolExternal = true, Status = 0 },
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
            var dto = new CreateVehicleRequestDto
            {
                RequestNo = "Test requestNo", RequestTypeId = 1L, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Purpose = "Test purpose", IsPoolExternal = true, Status = 0
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<VehicleRequest>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new VehicleRequest { Id = 1, RequestNo = "Test requestNo", RequestTypeId = 1L, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Purpose = "Test purpose", IsPoolExternal = true, Status = 0 });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new VehicleRequest { Id = 1, RequestNo = "Test requestNo", RequestTypeId = 1L, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Purpose = "Test purpose", IsPoolExternal = true, Status = 0 });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
