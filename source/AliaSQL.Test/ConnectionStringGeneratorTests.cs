using AliaSQL.Core.Model;
using AliaSQL.Core.Services.Impl;
using FluentAssertions;
using Xunit;

namespace AliaSQL.Test
{
    public class ConnectionStringGeneratorTests
    {
        [Fact]
        public void CorrectlyGeneratesConnectionStringWithDatabaseAndIntegratedSecurity()
        {
            var generator = new ConnectionStringGenerator();
            var settings = new ConnectionSettings("server", "db", true, string.Empty, string.Empty);
            var connectionString = generator.GetConnectionString(settings, true);

            connectionString.Should().Be("Data Source=server;Initial Catalog=db;Integrated Security=True;");
        }

        [Fact]
        public void CorrectlyGeneratesConnectionStringWithDatabaseAndIntegratedSecurityAndDatabaseExcluded()
        {
            var generator = new ConnectionStringGenerator();
            var settings = new ConnectionSettings("server", "db", true, string.Empty, string.Empty);
            var connectionString = generator.GetConnectionString(settings, false);

            connectionString.Should().Be("Data Source=server;Integrated Security=True;");
        }

        [Fact]
        public void CorrectlyGeneratesConnectionStringWithDatabaseAndUserSecurity()
        {
            var generator = new ConnectionStringGenerator();
            var settings = new ConnectionSettings("server", "db", false, "usr", "pwd");
            var connectionString = generator.GetConnectionString(settings, true);

            connectionString.Should().Be("Data Source=server;Initial Catalog=db;User ID=usr;Password=pwd;");
        }

        [Fact]
        public void CorrectlyGeneratesConnectionStringWithDatabaseAndUserSecurityAndDatabaseExcluded()
        {
            var generator = new ConnectionStringGenerator();
            var settings = new ConnectionSettings("server", "db", false, "usr", "pwd");
            var connectionString = generator.GetConnectionString(settings, false);

            connectionString.Should().Be("Data Source=server;User ID=usr;Password=pwd;");
        }
    }
}