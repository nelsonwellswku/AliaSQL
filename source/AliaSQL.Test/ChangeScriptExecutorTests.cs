using AliaSQL.Core.Model;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;
using NSubstitute;
using Xunit;

namespace AliaSQL.Test
{
    public class ChangeScriptExecutorTests
    {
        [Fact]
        public void LogsWarningsWhenScriptHasAlreadyBeenExecuted()
        {
            // Arrange
            var connectionSettings  = new ConnectionSettings(
                string.Empty,
                string.Empty,
                false,
                string.Empty,
                string.Empty
            );

            var scriptFile = @"C:\scripts\Update\01_Test.sql";
            var scriptExecutionTrackerMock = Substitute.For<IScriptExecutionTracker>();
            scriptExecutionTrackerMock.ScriptAlreadyExecuted(connectionSettings, "01_Test.sql").Returns(true);

            var taskObserverMock = Substitute.For<ITaskObserver>();

            // Act
            var executer = new ChangeScriptExecutor(scriptExecutionTrackerMock, null, null);
            executer.Execute(scriptFile, connectionSettings, taskObserverMock);

            // Assert
            scriptExecutionTrackerMock.Received().ScriptAlreadyExecuted(connectionSettings, "01_Test.sql");
            taskObserverMock.Received().Log("Skipping (already executed): 01_Test.sql");
        }
    }
}