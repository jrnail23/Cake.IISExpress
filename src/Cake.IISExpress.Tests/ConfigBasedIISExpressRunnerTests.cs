using System;
using Cake.Core;
using Cake.Core.Diagnostics;
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

            sut.StartServer(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(), Arg.Is<ProcessSettings>(p => p.Arguments.Render() == ""));
        }

        [Theory, CustomAutoData(typeof (IISExpressRunnerCustomizations))]
        public void ShouldSetSiteSwitch([Frozen] IProcessRunner runner,
            ConfigBasedIISExpressRunner sut)
        {
            var settings = new ConfigBasedIISExpressSettings { SiteNameToLaunch = "My Web Site" };

            sut.StartServer(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<ProcessSettings>(p => p.Arguments.Render() == "/site:\"My Web Site\""));
        }

        [Theory, CustomAutoData(typeof(IISExpressRunnerCustomizations))]
        public void ShouldSetAppPoolSwitch([Frozen] IProcessRunner runner,
    ConfigBasedIISExpressRunner sut)
        {
            var settings = new ConfigBasedIISExpressSettings { AppPoolToLaunch = "MyAppPool" };

            sut.StartServer(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<ProcessSettings>(p => p.Arguments.Render() == "/apppool:MyAppPool"));
        }

        [Theory, CustomAutoData(typeof (IISExpressRunnerCustomizations))]
        public void ShouldSetSiteIdSwitch([Frozen] IProcessRunner runner,
            ConfigBasedIISExpressRunner sut)
        {
            var settings = new ConfigBasedIISExpressSettings { SiteIdToLaunch = 53 };

            sut.StartServer(settings);

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

            sut.Invoking(s => s.StartServer(settings)).ShouldThrow<InvalidOperationException>();
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
                        f.FullPath.Equals("c:/MyWorkingDirectory/applicationhost.config",
                            StringComparison.OrdinalIgnoreCase))).Returns(true);

            sut.StartServer(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<ProcessSettings>(
                        p =>
                            p.Arguments.Render() ==
                            "/config:\"c:/MyWorkingDirectory/applicationhost.config\""));
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

            sut.StartServer(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<ProcessSettings>(
                        p =>
                            p.Arguments.Render() ==
                            "/config:\"c:/someOtherDirectory/applicationhost.config\""));
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

            sut.Invoking(s => s.StartServer(settings)).ShouldThrow<CakeException>();

            runner.DidNotReceiveWithAnyArgs().Start(null, null);
        }

        [Theory, CustomAutoData(typeof (IISExpressRunnerCustomizations))]
        public void ShouldSetSystraySwitch([Frozen] IProcessRunner runner,
            ConfigBasedIISExpressRunner sut)
        {
            var settings = new ConfigBasedIISExpressSettings { EnableSystemTray = false };

            sut.StartServer(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<ProcessSettings>(p => p.Arguments.Render() == "/systray:false"));
        }

        [Theory, CustomAutoData(typeof(IISExpressRunnerCustomizations))]
        public void ShouldSetTraceSwitch([Frozen] IProcessRunner runner,
            ConfigBasedIISExpressRunner sut)
        {
            var settings = new ConfigBasedIISExpressSettings { TraceLevel = TraceLevel.Warning };

            sut.StartServer(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<ProcessSettings>(p => p.Arguments.Render() == "/trace:warning"));
        }

        [Theory, CustomAutoData(typeof (IISExpressRunnerCustomizations))]
        public void ShouldThrowWhenIISExpressProcessWritesToErrorStream([Frozen(As = typeof(IProcess))] FakeProcess process,
            [Frozen] IProcessRunner processRunner, [Frozen] IRegistry registry,
            ConfigBasedIISExpressRunner sut)
        {
            processRunner.Start(Arg.Any<FilePath>(), Arg.Any<ProcessSettings>()).Returns(process);

            var settings = new ConfigBasedIISExpressSettings();

            sut.StartServer(settings);

            process.Invoking(
                p =>
                    p.TriggerErrorOutput("some dummy error data received"))
                .ShouldThrow<CakeException>()
                .WithMessage(
                    "IIS Express returned the following error message: 'some dummy error data received'");
        }

        [Theory, CustomAutoData(typeof (IISExpressRunnerCustomizations))]
        public void ShouldWaitUntilIISExpressServerIsStarted([Frozen]ICakeLog log, [Frozen(As = typeof(IProcess))] FakeProcess process,
            [Frozen] IProcessRunner processRunner, [Frozen] IRegistry registry,
            ConfigBasedIISExpressRunner sut)
        {
            var simulatedStandardOutput = new[]
            { "1", "2", "3", "4", "IIS Express is running.", "5" };

            // hooking into the logging call that occurs previous to waiting is the only place I could 
            // think of to send in simulated output to signal IIS Express has started.
            log.When(
                l =>
                    l.Write(Arg.Any<Verbosity>(), Arg.Any<LogLevel>(),
                        "Waiting for IIS Express to start (timeout: {0}ms)", Arg.Any<object[]>()))
                .Do(ci =>
                {
                    foreach (var s in simulatedStandardOutput)
                    {
                        process.TriggerStandardOutput(s);
                    }
                });

            processRunner.Start(Arg.Any<FilePath>(), Arg.Any<ProcessSettings>())
                .Returns(ci => process);

            var settings = new ConfigBasedIISExpressSettings { WaitForStartup = 1000 };

            sut.StartServer(settings);

            log.Received()
                .Write(Verbosity.Normal, LogLevel.Information,
                    Arg.Is<string>(s => s.StartsWith("IIS Express is running")), Arg.Any<object[]>());
        }

        [Theory, CustomAutoData(typeof(IISExpressRunnerCustomizations))]
        public void ShouldTimeoutWhenIISExpressTakesLongerThanSpecifiedWaitTimeToStart([Frozen]ICakeLog log, [Frozen(As = typeof(IProcess))] FakeProcess process,
    [Frozen] IProcessRunner processRunner, [Frozen] IRegistry registry,
    ConfigBasedIISExpressRunner sut)
        {
            var simulatedStandardOutput = new[]
            { "1", "2", "3", "4", "IIS Express is running.", "5" };

            // hooking into the logging call that occurs previous to waiting is the only place I could 
            // think of to send in simulated output to signal IIS Express has started.
            log.When(
                l =>
                    l.Write(Arg.Any<Verbosity>(), Arg.Any<LogLevel>(),
                        "Waiting for IIS Express to start (timeout: {0}ms)", Arg.Any<object[]>()))
                .Do(ci =>
                {
                    System.Threading.Thread.Sleep(100);
                    foreach (var s in simulatedStandardOutput)
                    {
                        process.TriggerStandardOutput(s);
                    }
                });

            processRunner.Start(Arg.Any<FilePath>(), Arg.Any<ProcessSettings>())
                .Returns(ci => process);

            var settings = new ConfigBasedIISExpressSettings { WaitForStartup = 50 };

            sut.Invoking(s => s.StartServer(settings))
                .ShouldThrow<CakeException>()
                .WithMessage("Timed out while waiting for IIS Express to start. (timeout: 50ms)");
        }
    }
}