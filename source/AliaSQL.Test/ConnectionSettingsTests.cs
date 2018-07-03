using AliaSQL.Core.Model;
using FluentAssertions;
using Xunit;

namespace AliaSQL.Test
{
    public class ConnectionSettingsTests
    {
        [Fact]
        public void PropertyAccessorsWork()
        {
            // Act
            var connectionSettings = new ConnectionSettings("server", "database", true, "username", "password");

            // Assert
            connectionSettings.Server.Should().Be("server");
            connectionSettings.Database.Should().Be("database");
            connectionSettings.IntegratedAuthentication.Should().BeTrue();
            connectionSettings.Username.Should().Be("username");
            connectionSettings.Password.Should().Be("password");
        }

        [Fact]
        public void ProperlyComparesTwoIdenticalConnectionSettings()
        {
            // Arrange
            var connectionSettingsOne = new ConnectionSettings("server", "database", true, "username", "password");
            var connectionSettingsTwo = new ConnectionSettings("server", "database", true, "username", "password");

            // Act
            var areEqual = connectionSettingsOne.Equals(connectionSettingsTwo);

            // Assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void ProperlyComparesTwoNonIdenticalConnectionSettings()
        {
            // Arrange
            var connectionSettingsOne = new ConnectionSettings("serverOne", "database", true, "username", "password");
            var connectionSettingsTwo = new ConnectionSettings("serverTwo", "database", true, "username", "password");

            // Act
            var areEqual = connectionSettingsOne.Equals(connectionSettingsTwo);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void CalculatesCorrectHashCode()
        { 
            // Arrange
            var expectedHashCode = "ServerDatabaseUsernamePasswordTrue".GetHashCode();

            // Act
            var connectionSettings = new ConnectionSettings("Server", "Database", true, "Username", "Password");

            // Assert
            connectionSettings.GetHashCode().Should().Be(expectedHashCode);
        }
    }
}