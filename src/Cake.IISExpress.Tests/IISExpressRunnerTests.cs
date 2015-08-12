using System;
using System.Collections.Generic;
using Cake.Core;
using Cake.Core.IO;
using FluentAssertions;
using NSubstitute;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

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
        public void AlternativeToolPathsShouldComeFromRegistry(IISExpressSettingsTestImpl dummySettings,
            [Frozen] IRegistry registry,
            IISExpressRunnerTestImpl sut)
        {
            registry.LocalMachine.OpenKey(@"SOFTWARE\Microsoft\IISExpress\8.0")
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

        public class IISExpressRunnerTestImpl : IISExpressRunner<IISExpressSettingsTestImpl>
        {
            public IISExpressRunnerTestImpl(IFileSystem fileSystem, ICakeEnvironment environment,
                IProcessRunner processRunner, IGlobber globber, IRegistry registry)
                : base(fileSystem, environment, processRunner, globber, registry)
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

            public IEnumerable<FilePath> Access_GetAlternativeToolPaths(IISExpressSettingsTestImpl settings)
            {
                return GetAlternativeToolPaths(settings);
            }

            public override IProcess RunProcess(IISExpressSettingsTestImpl settings)
            {
                throw new System.NotImplementedException();
            }
        }

        public class IISExpressSettingsTestImpl : IISExpressSettings
        {
             
        }
    }
}