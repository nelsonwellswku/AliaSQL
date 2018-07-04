using System;
using System.Collections.Generic;
using System.Text;
using AliaSQL.Core;
using AliaSQL.Core.Services;
using AliaSQL.Core.Services.Impl;
using AliaSQL.Core.Services.Impl.AliaSQL.Core.Services.Impl;
using FluentAssertions;
using Xunit;

namespace AliaSQL.Test
{
    public class DatabaseActionResolverTests
    {
        [Fact]
        public void CorrectlyDeterminesCreateActions()
        {
            // Arrange
            var resolver = new DatabaseActionResolver();

            // Act
            var actions = resolver.GetActions(RequestedDatabaseAction.Create);

            // Assert
            actions.Should().ContainInOrder(new List<DatabaseAction> {DatabaseAction.Create, DatabaseAction.Update});
        }

        [Fact]
        public void CorrectlyDeterminesUpdateActions()
        {
            // Arrange
            var resolver = new DatabaseActionResolver();

            // Act
            var actions = resolver.GetActions(RequestedDatabaseAction.Update);

            // Assert
            actions.Should().ContainInOrder(new List<DatabaseAction> { DatabaseAction.Update });
        }

        [Fact]
        public void CorrectlyDeterminesDropActions()
        {
            // Arrange
            var resolver = new DatabaseActionResolver();

            // Act
            var actions = resolver.GetActions(RequestedDatabaseAction.Drop);

            // Assert
            actions.Should().ContainInOrder(new List<DatabaseAction> { DatabaseAction.Drop });
        }

        [Fact]
        public void CorrectlyDeterminesRebuildActions()
        {
            // Arrange
            var resolver = new DatabaseActionResolver();

            // Act
            var actions = resolver.GetActions(RequestedDatabaseAction.Rebuild);

            // Assert
            actions.Should().ContainInOrder(new List<DatabaseAction> { DatabaseAction.Drop, DatabaseAction.Create, DatabaseAction.Update });
        }
    }
}
