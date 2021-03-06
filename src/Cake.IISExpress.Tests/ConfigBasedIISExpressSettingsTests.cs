using FluentAssertions;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Cake.IISExpress.Tests
{
    public class ConfigBasedIISExpressSettingsTests
    {
        [Theory, CustomAutoData]
        public void EnableSystemTrayDefaultsToTrue(
            [NoAutoProperties] ConfigBasedIISExpressSettings sut)
        {
            sut.EnableSystemTray.Should().BeTrue();
        }

        [Theory, CustomAutoData]
        public void TraceLevelDefaultsToNone([NoAutoProperties] ConfigBasedIISExpressSettings sut)
        {
            sut.TraceLevel.Should().Be(TraceLevel.None);
        }
    }
}