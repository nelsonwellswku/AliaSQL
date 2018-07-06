using AliaSQL.Core;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;
using NSubstitute;
using Xunit;

namespace AliaSQL.Test
{
    public class DatabaseCreatorTests
    {
        [Fact]
        public void CreatesDatabase()
        {
            // Arrange
            var connectionSettings = new ConnectionSettings("server", "db", true, null, null);
            var taskAttributes = new TaskAttributes(connectionSettings, @"c:\scripts")
            {
                RequestedDatabaseAction = RequestedDatabaseAction.Create
            };

            var queryExecutorMock = Substitute.For<IQueryExecutor>();
            var scriptFolderExecuturerMock = Substitute.For<IScriptFolderExecutor>();
            var taskObserver = Substitute.For<ITaskObserver>();

            // Act
            var creator = new DatabaseCreator(queryExecutorMock, scriptFolderExecuturerMock);
            creator.Execute(taskAttributes, taskObserver);

            // Assert
            queryExecutorMock.Received().ExecuteNonQuery(connectionSettings, "create database [db]");
            taskObserver.Received().Log("Run scripts in Create folder.");
            scriptFolderExecuturerMock.Received().ExecuteScriptsInFolder(taskAttributes, "Create", taskObserver);
        }
    }
}