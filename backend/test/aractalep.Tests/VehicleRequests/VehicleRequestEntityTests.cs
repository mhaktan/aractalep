using System;
using Xunit;
using FluentAssertions;
using aractalep.Entities;

namespace aractalep.Tests.VehicleRequests
{
    public class VehicleRequestEntityTests
    {
        [Fact]
        public void VehicleRequest_ShouldBeCreatable()
        {
            // Act
            var entity = new VehicleRequest();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void VehicleRequest_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new VehicleRequest();

            // Assert
            entity.Id.Should().Be(default(long));
            entity.IsPoolExternal.Should().Be(false);
        }

        [Fact]
        public void VehicleRequest_RequestNo_ShouldAcceptValue()
        {
            var entity = new VehicleRequest { RequestNo = "Test Value" };
            entity.RequestNo.Should().Be("Test Value");
        }

        [Fact]
        public void VehicleRequest_Purpose_ShouldAcceptValue()
        {
            var entity = new VehicleRequest { Purpose = "Test Value" };
            entity.Purpose.Should().Be("Test Value");
        }

    }
}
