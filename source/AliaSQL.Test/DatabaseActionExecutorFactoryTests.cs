using System.Collections.Generic;
using System.Linq;
using AliaSQL.Core;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;
using AliaSQL.Core.Services.Impl.AliaSQL.Core.Services.Impl;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace AliaSQL.Test
{
    public class DatabaseActionExecutorFactoryTests
    {
        [Fact]
        public void CorrectlyConstructsActionExecutors()
        {
            // Arrange
            var actions = new[] {DatabaseAction.Create, DatabaseAction.Update};
            var resolverMock = Substitute.For<IDatabaseActionResolver>();
            var locatorMock = Substitute.For<IDataBaseActionLocator>();
            var creator = Substitute.For<IDatabaseActionExecutor>();
            var updater = Substitute.For<IDatabaseActionExecutor>();

            resolverMock.GetActions(RequestedDatabaseAction.Create).Returns(actions);
            locatorMock.CreateInstance(DatabaseAction.Create).Returns(creator);
            locatorMock.CreateInstance(DatabaseAction.Update).Returns(updater);

            // Act
            var factory = new DatabaseActionExecutorFactory(resolverMock, locatorMock);
            var executors = factory.GetExecutors(RequestedDatabaseAction.Create).ToList();

            // Assert
            var expected = new List<IDatabaseActionExecutor> {creator, updater};
            executors.Should().ContainInOrder(expected);
        }
    }
}