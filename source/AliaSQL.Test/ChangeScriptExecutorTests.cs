using AliaSQL.Core;
using AliaSQL.Core.Model;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;
using NSubstitute;
using Xunit;

namespace AliaSQL.Test
{
    public class ChangeScriptExecutorTests
    {
        private static ConnectionSettings ConnectionFake => new ConnectionSettings(
            string.Empty,
            string.Empty,
            false,
            string.Empty,
            string.Empty
        );

        private const string ScriptFile = @"C:\scripts\Update\01_Test.sql";

        [Fact]
        public void LogsWarningsWhenScriptHasAlreadyBeenExecuted()
        {
            // Arrange
            var connectionSettings = ConnectionFake;

            var scriptExecutionTrackerMock = Substitute.For<IScriptExecutionTracker>();
            scriptExecutionTrackerMock.ScriptAlreadyExecuted(connectionSettings, "01_Test.sql").Returns(true);

            var taskObserverMock = Substitute.For<ITaskObserver>();

            // Act
            var executer = new ChangeScriptExecutor(scriptExecutionTrackerMock, null, null);
            executer.Execute(ScriptFile, connectionSettings, taskObserverMock);

            // Assert
            scriptExecutionTrackerMock.Received().ScriptAlreadyExecuted(connectionSettings, "01_Test.sql");
            taskObserverMock.Received().Log("Skipping (already executed): 01_Test.sql");
        }

        [Fact]
        public void ExecutesScriptIfItHasNotAlreadyBeenExecuted()
        {
            // Arrange
            var connectionSettings = ConnectionFake;
            const string fileContents = "file contents...";

            var scriptExecutionTrackerMock = Substitute.For<IScriptExecutionTracker>();
            var fileSystemMock = Substitute.For<IFileSystem>();
            var queryExecuterMock = Substitute.For<IQueryExecutor>();
            var taskObserverMock = Substitute.For<ITaskObserver>();

            scriptExecutionTrackerMock.ScriptAlreadyExecuted(connectionSettings, "01_Test.sql").Returns(false);
            fileSystemMock.ReadTextFile(ScriptFile).Returns(fileContents);
            queryExecuterMock.ScriptSupportsTransactions(fileContents).Returns(true);

            // Act
            var executer = new ChangeScriptExecutor(scriptExecutionTrackerMock, queryExecuterMock, fileSystemMock);
            executer.Execute(ScriptFile, connectionSettings, taskObserverMock);

            // Assert
            taskObserverMock.Received().Log("Executing: 01_Test.sql in a transaction");
            queryExecuterMock.Received().ExecuteNonQueryTransactional(connectionSettings, fileContents);
            scriptExecutionTrackerMock.Received().MarkScriptAsExecuted(connectionSettings, "01_Test.sql", taskObserverMock);
        }

        [Fact]
        public void ExecutesScriptsWithTransactionStopwordsOutsideOfATransaction()
        {
            // Arrange
            var connectionSettings = ConnectionFake;
            const string fileContents = "CREATE DATABASE ...";

            var scriptExecutionTrackerMock = Substitute.For<IScriptExecutionTracker>();
            var fileSystemMock = Substitute.For<IFileSystem>();
            var queryExecuterMock = Substitute.For<IQueryExecutor>();
            var taskObserverMock = Substitute.For<ITaskObserver>();

            scriptExecutionTrackerMock.ScriptAlreadyExecuted(connectionSettings, "01_Test.sql").Returns(false);
            fileSystemMock.ReadTextFile(ScriptFile).Returns(fileContents);
            queryExecuterMock.ScriptSupportsTransactions(fileContents).Returns(false);

            // Act
            var executer = new ChangeScriptExecutor(scriptExecutionTrackerMock, queryExecuterMock, fileSystemMock);
            executer.Execute(ScriptFile, connectionSettings, taskObserverMock);

            // Assert
            taskObserverMock.Received().Log("Executing: 01_Test.sql");
            queryExecuterMock.Received().ExecuteNonQuery(connectionSettings, fileContents, true);
            scriptExecutionTrackerMock.Received().MarkScriptAsExecuted(connectionSettings, "01_Test.sql", taskObserverMock);
        }
    }
}