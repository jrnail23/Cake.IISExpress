using FluentAssertions;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Cake.IISExpress.Tests
{
    public class AppPathBasedIISExpressSettingsTests
    {
        [Theory, CustomAutoData]
        public void EnableSystemTrayDefaultsToTrue(
            [NoAutoProperties] AppPathBasedIISExpressSettings sut)
        {
            sut.EnableSystemTray.Should().BeTrue();
        }

        [Theory, CustomAutoData]
        public void TraceLevelDefaultsToNone([NoAutoProperties] AppPathBasedIISExpressSettings sut)
        {
            sut.TraceLevel.Should().Be(TraceLevel.None);
        }
    }
}