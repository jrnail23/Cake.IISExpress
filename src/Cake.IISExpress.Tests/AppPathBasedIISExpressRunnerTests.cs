﻿using System;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Utilities;
using Cake.Process;
using FluentAssertions;
using NSubstitute;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Cake.IISExpress.Tests
{
    public class AppPathBasedIISExpressRunnerTests
    {
        [Theory, CustomAutoData]
        public void ShouldImplementTool(AppPathBasedIISExpressRunner sut)
        {
            sut.Should().BeAssignableTo<Tool<AppPathBasedIISExpressSettings>>();
        }

        [Theory, CustomAutoData(typeof (IISExpressRunnerCustomizations))]
        public void ShouldThrowIfAppPathDoesNotExist([Frozen] IAdvProcessRunner runner,
            IFileSystem fileSystem,
            AppPathBasedIISExpressRunner sut)
        {
            var settings = new AppPathBasedIISExpressSettings(@"c:\MyApp");

            fileSystem.Exist(settings.AppPath).Returns(false);

            sut.Invoking(s => s.StartServer(settings)).ShouldThrow<CakeException>();

            runner.DidNotReceiveWithAnyArgs()
                .Start(null, null);
        }

        [Theory, CustomAutoData(typeof(IISExpressRunnerCustomizations))]
        public void ShouldSetAppPathSwitchFromAbsolutePath(
            [Frozen] ICakeEnvironment environment,
            IFileSystem fileSystem, [Frozen] IAdvProcessRunner runner,
            AppPathBasedIISExpressRunner sut)
        {
            var settings = new AppPathBasedIISExpressSettings(@"c:\MyApp");
            
            fileSystem.Exist(Arg.Is(settings.AppPath)).Returns(true);

            sut.StartServer(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<AdvProcessSettings>(
                        p =>
                            p.Arguments.Render() ==
                            "/path:\"c:/MyApp\""));
        }

        [Theory, CustomAutoData(typeof(IISExpressRunnerCustomizations))]
        public void ShouldSetAppPathSwitchFromRelativeFilePath(
            [Frozen] ICakeEnvironment environment,
            IFileSystem fileSystem, [Frozen] IAdvProcessRunner runner,
            AppPathBasedIISExpressRunner sut)
        {
            environment.WorkingDirectory.Returns("c:/build/MyWorkingDirectory");

            var settings = new AppPathBasedIISExpressSettings(@"..\MyApp");

            fileSystem.Exist(Arg.Is<DirectoryPath>(x => x.FullPath == "c:/build/MyApp")).Returns(true);

            sut.StartServer(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<AdvProcessSettings>(
                        p =>
                            p.Arguments.Render() ==
                            "/path:\"c:/build/MyApp\""));
        }

        [Theory, CustomAutoData(typeof(IISExpressRunnerCustomizations))]
        public void ShouldSetSystraySwitch([Frozen] IAdvProcessRunner runner,
             IFileSystem fileSystem, AppPathBasedIISExpressRunner sut)
        {
            var settings = new AppPathBasedIISExpressSettings(@"c:\MyApp") { EnableSystemTray = false };
            fileSystem.Exist(settings.AppPath).Returns(true);
            sut.StartServer(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<AdvProcessSettings>(p => p.Arguments.Render() == "/path:\"c:/MyApp\" /systray:false"));
        }

        [Theory, CustomAutoData(typeof(IISExpressRunnerCustomizations))]
        public void ShouldSetTraceSwitch([Frozen] IAdvProcessRunner runner,
             IFileSystem fileSystem, AppPathBasedIISExpressRunner sut)
        {
            var settings = new AppPathBasedIISExpressSettings(@"c:\MyApp") { TraceLevel = TraceLevel.Warning };
            fileSystem.Exist(settings.AppPath).Returns(true);

            sut.StartServer(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<AdvProcessSettings>(p => p.Arguments.Render() == "/path:\"c:/MyApp\" /trace:warning"));
        }

        [Theory, CustomAutoData(typeof(IISExpressRunnerCustomizations))]
        public void ShouldSetPortSwitch([Frozen] IAdvProcessRunner runner,
             IFileSystem fileSystem, AppPathBasedIISExpressRunner sut)
        {
            var settings = new AppPathBasedIISExpressSettings(@"c:\MyApp") { PortNumber = 5555 };
            fileSystem.Exist(settings.AppPath).Returns(true);

            sut.StartServer(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<AdvProcessSettings>(
                        p => p.Arguments.Render() == "/path:\"c:/MyApp\" /port:5555"));
        }

        [Theory, CustomAutoData(typeof(IISExpressRunnerCustomizations))]
        public void ShouldSetClrVersionSwitch([Frozen] IAdvProcessRunner runner,
             IFileSystem fileSystem, AppPathBasedIISExpressRunner sut)
        {
            var settings = new AppPathBasedIISExpressSettings(@"c:\MyApp") { ClrVersion = ClrVersion.Version20 };
            fileSystem.Exist(settings.AppPath).Returns(true);

            sut.StartServer(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<AdvProcessSettings>(
                        p => p.Arguments.Render() == "/path:\"c:/MyApp\" /clr:v2.0"));
        }

        [Theory, CustomAutoData(typeof (IISExpressRunnerCustomizations))]
        public void ShouldThrowWhenIISExpressProcessWritesToErrorStream([Frozen] IAdvProcess process,
            IFileSystem fileSystem,
            [Frozen] IAdvProcessRunner processRunner, [Frozen] IRegistry registry,
            AppPathBasedIISExpressRunner sut)
        {
            processRunner.Start(Arg.Any<FilePath>(), Arg.Any<AdvProcessSettings>()).Returns(process);

            var settings = new AppPathBasedIISExpressSettings(@"c:\MyApp");
            fileSystem.Exist(settings.AppPath).Returns(true);

            sut.StartServer(settings);

            process.Invoking(
                           p =>
                               p.ErrorDataReceived +=
                                   Raise.EventWith(
                                       new ProcessOutputReceivedEventArgs("some dummy error data received")))
                           .ShouldThrow<CakeException>()
                           .WithMessage(
                               "IIS Express returned the following error message: 'some dummy error data received'");
        }

        [Theory, CustomAutoData(typeof (IISExpressRunnerCustomizations))]
        public void ShouldWaitUntilIISExpressServerIsStarted([Frozen] ICakeLog log,
            [Frozen] IAdvProcess process, IFileSystem fileSystem,
            [Frozen] IAdvProcessRunner processRunner, [Frozen] IRegistry registry,
            AppPathBasedIISExpressRunner sut)
        {
            var simulatedStandardOutput = new[]
            {"1", "2", "3", "4", "IIS Express is running.", "5"};

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
                        process.OutputDataReceived += Raise.EventWith(process,
                            new ProcessOutputReceivedEventArgs(s));
                    }
                });

            processRunner.Start(Arg.Any<FilePath>(), Arg.Any<AdvProcessSettings>())
                .Returns(ci => process);

            var settings = new AppPathBasedIISExpressSettings(@"c:\MyApp") {WaitForStartup = 1000};
            fileSystem.Exist(settings.AppPath).Returns(true);

            sut.StartServer(settings);

            log.Received()
                .Write(Verbosity.Normal, LogLevel.Information,
                    Arg.Is<string>(s => s.StartsWith("IIS Express is running")), Arg.Any<object[]>());
        }
    }
}