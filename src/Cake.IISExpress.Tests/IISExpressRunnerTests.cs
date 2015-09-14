using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Process;
using FluentAssertions;
using NSubstitute;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Cake.IISExpress.Tests
{
    public class IISExpressRunnerTests
    {
        [Theory, CustomAutoData]
        public void GetToolExecutableNamesReturnsEmpty(IISExpressRunnerTestImpl sut)
        {
            var result = sut.Access_GetToolExecutableNames();
            result.Should().BeEmpty();
        }

        [Theory, CustomAutoData]
        public void ToolNameIsIISExpress(IISExpressRunnerTestImpl sut)
        {
            var result = sut.Access_GetToolName();
            result.Should().Be("IISExpress");
        }

        [Theory, CustomAutoData]
        public void AlternativeToolPathsShouldComeFromRegistry(
            IISExpressSettingsTestImpl dummySettings,
            [Frozen] IRegistry registry,
            IISExpressRunnerTestImpl sut)
        {
            registry.LocalMachine.OpenKey(@"SOFTWARE\Microsoft\IISExpress")
                .GetSubKeyNames()
                .Returns(new[] { "8.0", "10.0" });
            registry.LocalMachine.OpenKey(@"SOFTWARE\Microsoft\IISExpress")
                .OpenKey("10.0")
                .GetValue("InstallPath")
                .Returns("MyIISExpressInstallPath");

            var result = sut.Access_GetAlternativeToolPaths(dummySettings);

            result.Should()
                .HaveCount(1)
                .And.ContainSingle(
                    r =>
                        r.FullPath.Equals(@"MyIISExpressInstallPath/IISExpress.exe",
                            StringComparison.OrdinalIgnoreCase));
        }

        [Theory, CustomAutoData]
        public void AlternativeToolPathsThrowsWhenRegistryItemDoesNotExist(
            IISExpressSettingsTestImpl dummySettings,
            [Frozen] IRegistry registry,
            IISExpressRunnerTestImpl sut)
        {
            registry.LocalMachine.OpenKey(@"SOFTWARE\Microsoft\IISExpress")
                .Returns(null as IRegistryKey);

            sut.Invoking(_ => _.Access_GetAlternativeToolPaths(dummySettings).ToArray())
                .ShouldThrow<CakeException>()
                .WithMessage("IIS Express is not installed on this machine.");
        }

        public class IISExpressRunnerTestImpl : IISExpressRunner<IISExpressSettingsTestImpl>
        {
            public IISExpressRunnerTestImpl(IFileSystem fileSystem, ICakeEnvironment environment,
                IProcessRunner processRunner, IGlobber globber, IRegistry registry, ICakeLog log,
                IAdvProcessRunner advProcessRunner)
                : base(
                    fileSystem, environment, processRunner, globber, registry, log, advProcessRunner
                    )
            {
            }

            public string Access_GetToolName()
            {
                return GetToolName();
            }

            public IEnumerable<string> Access_GetToolExecutableNames()
            {
                return GetToolExecutableNames();
            }

            public IEnumerable<FilePath> Access_GetAlternativeToolPaths(
                IISExpressSettingsTestImpl settings)
            {
                return GetAlternativeToolPaths(settings);
            }

            protected override ProcessArgumentBuilder BuildArguments(
                IISExpressSettingsTestImpl settings)
            {
                return new ProcessArgumentBuilder();
            }
        }

        public class IISExpressSettingsTestImpl : IISExpressSettings
        {
        }
    }
}