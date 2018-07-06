using System;
using System.Collections.Generic;
using System.Text;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;
using NSubstitute;
using Xunit;

namespace AliaSQL.Test
{
    public class DatabaseDropperTests
    {
        [Fact]
        public void DropsDatabase()
        {
            // Arrange
            var connectionSettings = new ConnectionSettings("server", "db", true, null, null);
            var taskAttributes = new TaskAttributes(connectionSettings, null);

            var connectionDropperMock = Substitute.For<IDatabaseConnectionDropper>();
            var taskObserverMock = Substitute.For<ITaskObserver>();
            var queryExecuterMock = Substitute.For<IQueryExecutor>();

            queryExecuterMock.ReadFirstColumnAsStringArray(connectionSettings, "select @@version").Returns(new[] { "SQL Server" });

            // Act
            var dropper = new DatabaseDropper(connectionDropperMock, queryExecuterMock);
            dropper.Execute(taskAttributes, taskObserverMock);

            // Assert
            taskObserverMock.Received().Log("Running against: SQL Server");
            taskObserverMock.Received().Log("Dropping database: db\n");
            connectionDropperMock.Received().Drop(connectionSettings, taskObserverMock);
            queryExecuterMock.Received().ExecuteNonQuery(connectionSettings, "ALTER DATABASE [db] SET SINGLE_USER WITH ROLLBACK IMMEDIATE drop database [db]");
        }

        [Fact]
        public void DropsAzureDatabaseWithoutDroppingConnections()
        {
            // Arrange
            var settings = new ConnectionSettings("server", "db", true, null, null);
            var taskAttributes = new TaskAttributes(settings, null);

            var connectionDropperMock = Substitute.For<IDatabaseConnectionDropper>();
            var taskObserverMock = Substitute.For<ITaskObserver>();
            var queryExecutorMock = Substitute.For<IQueryExecutor>();

            queryExecutorMock.ReadFirstColumnAsStringArray(settings, "select @@version").Returns(new[] {"SQL Azure "});

            // Act
            var dropper = new DatabaseDropper(connectionDropperMock, queryExecutorMock);
            dropper.Execute(taskAttributes, taskObserverMock);

            // Assert
            taskObserverMock.Log("Running against: SQL Azure");
            taskObserverMock.Log("Dropping database: db\n");
            queryExecutorMock.Received().ExecuteNonQuery(settings, "drop database [db]");
            connectionDropperMock.DidNotReceiveWithAnyArgs().Drop(null, null);
        }

        [Fact]
        public void ShouldNotFailIfDatabaseDoesNotExist()
        {
            throw new NotImplementedException();
        }
    }
}
