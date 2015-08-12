using System;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Utilities;
using FluentAssertions;
using NSubstitute;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Cake.IISExpress.Tests
{
    public class ConfigBasedIISExpressRunnerTests
    {
        [Theory, CustomAutoData]
        public void ShouldImplementTool(ConfigBasedIISExpressRunner sut)
        {
            sut.Should().BeAssignableTo<Tool<ConfigBasedIISExpressSettings>>();
        }

        [Theory, CustomAutoData(typeof (IISExpressRunnerCustomizations))]
        public void ShouldNotSetAnySwitchesByDefault([Frozen] IProcessRunner runner,
            ConfigBasedIISExpressRunner sut)
        {
            var settings = new ConfigBasedIISExpressSettings();

            sut.RunProcess(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(), Arg.Is<ProcessSettings>(p => p.Arguments.Render() == ""));
        }

        [Theory, CustomAutoData(typeof (IISExpressRunnerCustomizations))]
        public void ShouldSetSiteSwitch([Frozen] IProcessRunner runner,
            ConfigBasedIISExpressRunner sut)
        {
            var settings = new ConfigBasedIISExpressSettings { SiteNameToLaunch = "My Web Site" };

            sut.RunProcess(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<ProcessSettings>(p => p.Arguments.Render() == "/site:'My Web Site'"));
        }

        [Theory, CustomAutoData(typeof (IISExpressRunnerCustomizations))]
        public void ShouldSetSiteIdSwitch([Frozen] IProcessRunner runner,
            ConfigBasedIISExpressRunner sut)
        {
            var settings = new ConfigBasedIISExpressSettings { SiteIdToLaunch = 53 };

            sut.RunProcess(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<ProcessSettings>(p => p.Arguments.Render() == "/siteid:53"));
        }

        [Theory, CustomAutoData(typeof (IISExpressRunnerCustomizations))]
        public void ShouldThrowWhenBothSiteIdAndSiteNameAreSet([Frozen] IProcessRunner runner,
            ConfigBasedIISExpressRunner sut)
        {
            var settings = new ConfigBasedIISExpressSettings
            {
                SiteIdToLaunch = 53,
                SiteNameToLaunch = "MySite"
            };

            sut.Invoking(s => s.RunProcess(settings)).ShouldThrow<InvalidOperationException>();
        }

        [Theory, CustomAutoData(typeof (IISExpressRunnerCustomizations))]
        public void ShouldSetConfigFileSwitchFromRelativeFilePath(
            [Frozen] ICakeEnvironment environment,
            IFileSystem fileSystem, [Frozen] IProcessRunner runner,
            ConfigBasedIISExpressRunner sut)
        {
            environment.WorkingDirectory.Returns("c:/MyWorkingDirectory");
            var settings = new ConfigBasedIISExpressSettings
            {
                ConfigFilePath = FilePath.FromString("applicationhost.config")
            };

            fileSystem.Exist(
                Arg.Is<FilePath>(
                    f =>
                        f.FullPath.Equals(settings.ConfigFilePath.FullPath,
                            StringComparison.OrdinalIgnoreCase))).Returns(true);

            sut.RunProcess(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<ProcessSettings>(
                        p =>
                            p.Arguments.Render() ==
                            "/config:'c:/MyWorkingDirectory/applicationhost.config'"));
        }

        [Theory, CustomAutoData(typeof (IISExpressRunnerCustomizations))]
        public void ShouldSetConfigFileSwitchFromAbsoluteFilePath(
            [Frozen] ICakeEnvironment environment,
            IFileSystem fileSystem, [Frozen] IProcessRunner runner,
            ConfigBasedIISExpressRunner sut)
        {
            environment.WorkingDirectory.Returns("c:/MyWorkingDirectory");
            var settings = new ConfigBasedIISExpressSettings
            {
                ConfigFilePath =
                    FilePath.FromString(@"c:\someOtherDirectory\applicationhost.config")
            };

            fileSystem.Exist(
                Arg.Is<FilePath>(
                    f =>
                        f.FullPath.Equals(settings.ConfigFilePath.FullPath,
                            StringComparison.OrdinalIgnoreCase))).Returns(true);

            sut.RunProcess(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<ProcessSettings>(
                        p =>
                            p.Arguments.Render() ==
                            "/config:'c:/someOtherDirectory/applicationhost.config'"));
        }

        [Theory, CustomAutoData(typeof (IISExpressRunnerCustomizations))]
        public void ShouldThrowWhenConfigFileDoesNotExist([Frozen] ICakeEnvironment environment,
            IFileSystem fileSystem, [Frozen] IProcessRunner runner,
            ConfigBasedIISExpressRunner sut)
        {
            environment.WorkingDirectory.Returns("c:/MyWorkingDirectory");

            var settings = new ConfigBasedIISExpressSettings
            {
                ConfigFilePath =
                    FilePath.FromString(@"c:\someOtherDirectory\applicationhost.config")
            };

            fileSystem.Exist(
                Arg.Is<FilePath>(
                    f =>
                        f.FullPath.Equals(settings.ConfigFilePath.FullPath,
                            StringComparison.OrdinalIgnoreCase))).Returns(false);

            sut.Invoking(s => s.RunProcess(settings)).ShouldThrow<CakeException>();

            runner.DidNotReceiveWithAnyArgs().Start(null, null);
        }

        [Theory, CustomAutoData(typeof (IISExpressRunnerCustomizations))]
        public void ShouldSetSystraySwitch([Frozen] IProcessRunner runner,
            ConfigBasedIISExpressRunner sut)
        {
            var settings = new ConfigBasedIISExpressSettings { EnableSystemTray = false };

            sut.RunProcess(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<ProcessSettings>(p => p.Arguments.Render() == "/systray:false"));
        }

        [Theory, CustomAutoData(typeof(IISExpressRunnerCustomizations))]
        public void ShouldSetTraceSwitch([Frozen] IProcessRunner runner,
            ConfigBasedIISExpressRunner sut)
        {
            var settings = new ConfigBasedIISExpressSettings { TraceLevel = TraceLevel.Warning };

            sut.RunProcess(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<ProcessSettings>(p => p.Arguments.Render() == "/trace:warning"));
        }
    }
}