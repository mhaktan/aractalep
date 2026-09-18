using System;
using Xunit;
using FluentAssertions;
using aractalep.Entities;

namespace aractalep.Tests.Vehicles
{
    public class VehicleEntityTests
    {
        [Fact]
        public void Vehicle_ShouldBeCreatable()
        {
            // Act
            var entity = new Vehicle();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void Vehicle_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new Vehicle();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void Vehicle_Plate_ShouldAcceptValue()
        {
            var entity = new Vehicle { Plate = "Test Value" };
            entity.Plate.Should().Be("Test Value");
        }

        [Fact]
        public void Vehicle_Brand_ShouldAcceptValue()
        {
            var entity = new Vehicle { Brand = "Test Value" };
            entity.Brand.Should().Be("Test Value");
        }

        [Fact]
        public void Vehicle_Model_ShouldAcceptValue()
        {
            var entity = new Vehicle { Model = "Test Value" };
            entity.Model.Should().Be("Test Value");
        }

    }
}
