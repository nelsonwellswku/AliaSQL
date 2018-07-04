using AliaSQL.Core;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;
using NSubstitute;
using Xunit;

namespace AliaSQL.Test
{
    public class DatabaseConnectionDropperTests
    {
        [Fact]
        public void CorrectlyDropsConnections()
        {
            // Arrange
            var assembly = SqlDatabaseManager.SQL_FILE_ASSEMBLY;
            var sqlFile = string.Format(SqlDatabaseManager.SQL_FILE_TEMPLATE, "DropConnections");
            var connectionSettings = new ConnectionSettings("server", "MyDatabase", true, null, null);

            var taskObserverMock = Substitute.For<ITaskObserver>();
            var fileLocatorMock = Substitute.For<IResourceFileLocator>();
            var replacerMock = Substitute.ForPartsOf<TokenReplacer>();
            replacerMock.Text = "";
            var queryExecuterMock = Substitute.For<IQueryExecutor>();

            fileLocatorMock.ReadTextFile(assembly, sqlFile).Returns("SQL: ||DatabaseName||");

            // Act
            var dropper = new DatabaseConnectionDropper(fileLocatorMock, replacerMock, queryExecuterMock);
            dropper.Drop(connectionSettings, taskObserverMock);

            // Assert
            taskObserverMock.Received().Log("Dropping connections for database MyDatabase\n");
            replacerMock.Received().Replace("DatabaseName", "MyDatabase");
            queryExecuterMock.Received().ExecuteNonQuery(connectionSettings, "SQL: MyDatabase");
        }
    }
}