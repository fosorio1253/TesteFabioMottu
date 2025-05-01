using FluentAssertions;
using Vrumm.Domain.Common;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Commom;
public class DomainEventTests
{
    private class TestDomainEvent : DomainEvent { }

    [Fact]
    public void Constructor_SetsIdAndTimestamp()
    {
        // Act
        var domainEvent = new TestDomainEvent();

        // Assert
        domainEvent.Id.Should().NotBe(Guid.Empty);
        domainEvent.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}