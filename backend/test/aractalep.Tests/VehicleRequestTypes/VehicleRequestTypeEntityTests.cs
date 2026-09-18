using System;
using Xunit;
using FluentAssertions;
using aractalep.Entities;

namespace aractalep.Tests.VehicleRequestTypes
{
    public class VehicleRequestTypeEntityTests
    {
        [Fact]
        public void VehicleRequestType_ShouldBeCreatable()
        {
            // Act
            var entity = new VehicleRequestType();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void VehicleRequestType_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new VehicleRequestType();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void VehicleRequestType_Name_ShouldAcceptValue()
        {
            var entity = new VehicleRequestType { Name = "Test Value" };
            entity.Name.Should().Be("Test Value");
        }

    }
}
