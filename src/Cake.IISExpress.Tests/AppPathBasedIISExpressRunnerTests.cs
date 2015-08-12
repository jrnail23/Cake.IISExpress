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
    public class AppPathBasedIISExpressRunnerTests
    {
        [Theory, CustomAutoData]
        public void ShouldImplementTool(AppPathBasedIISExpressRunner sut)
        {
            sut.Should().BeAssignableTo<Tool<AppPathBasedIISExpressSettings>>();
        }

        [Theory, CustomAutoData(typeof (IISExpressRunnerCustomizations))]
        public void ShouldThrowIfAppPathDoesNotExist([Frozen] IProcessRunner runner,
            IFileSystem fileSystem,
            AppPathBasedIISExpressRunner sut)
        {
            var settings = new AppPathBasedIISExpressSettings(@"c:\MyApp");

            fileSystem.Exist(settings.AppPath).Returns(false);

            sut.Invoking(s => s.RunProcess(settings)).ShouldThrow<CakeException>();

            runner.DidNotReceiveWithAnyArgs()
                .Start(null, null);
        }

        [Theory, CustomAutoData(typeof(IISExpressRunnerCustomizations))]
        public void ShouldSetAppPathSwitchFromAbsolutePath(
            [Frozen] ICakeEnvironment environment,
            IFileSystem fileSystem, [Frozen] IProcessRunner runner,
            AppPathBasedIISExpressRunner sut)
        {
            var settings = new AppPathBasedIISExpressSettings(@"c:\MyApp");
            
            fileSystem.Exist(Arg.Is(settings.AppPath)).Returns(true);

            sut.RunProcess(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<ProcessSettings>(
                        p =>
                            p.Arguments.Render() ==
                            "/path:'c:/MyApp'"));
        }

        [Theory, CustomAutoData(typeof(IISExpressRunnerCustomizations))]
        public void ShouldSetAppPathSwitchFromRelativeFilePath(
            [Frozen] ICakeEnvironment environment,
            IFileSystem fileSystem, [Frozen] IProcessRunner runner,
            AppPathBasedIISExpressRunner sut)
        {
            environment.WorkingDirectory.Returns("c:/build/MyWorkingDirectory");

            var settings = new AppPathBasedIISExpressSettings(@"..\MyApp");

            fileSystem.Exist(Arg.Is<DirectoryPath>(x => x.FullPath == "c:/build/MyApp")).Returns(true);

            sut.RunProcess(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<ProcessSettings>(
                        p =>
                            p.Arguments.Render() ==
                            "/path:'c:/build/MyApp'"));
        }

        [Theory, CustomAutoData(typeof(IISExpressRunnerCustomizations))]
        public void ShouldSetSystraySwitch([Frozen] IProcessRunner runner,
             IFileSystem fileSystem, AppPathBasedIISExpressRunner sut)
        {
            var settings = new AppPathBasedIISExpressSettings(@"c:\MyApp") { EnableSystemTray = false };
            fileSystem.Exist(settings.AppPath).Returns(true);
            sut.RunProcess(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<ProcessSettings>(p => p.Arguments.Render() == "/path:'c:/MyApp' /systray:false"));
        }

        [Theory, CustomAutoData(typeof(IISExpressRunnerCustomizations))]
        public void ShouldSetTraceSwitch([Frozen] IProcessRunner runner,
             IFileSystem fileSystem, AppPathBasedIISExpressRunner sut)
        {
            var settings = new AppPathBasedIISExpressSettings(@"c:\MyApp") { TraceLevel = TraceLevel.Warning };
            fileSystem.Exist(settings.AppPath).Returns(true);

            sut.RunProcess(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<ProcessSettings>(p => p.Arguments.Render() == "/path:'c:/MyApp' /trace:warning"));
        }

        [Theory, CustomAutoData(typeof(IISExpressRunnerCustomizations))]
        public void ShouldSetPortSwitch([Frozen] IProcessRunner runner,
             IFileSystem fileSystem, AppPathBasedIISExpressRunner sut)
        {
            var settings = new AppPathBasedIISExpressSettings(@"c:\MyApp") { PortNumber = 5555 };
            fileSystem.Exist(settings.AppPath).Returns(true);

            sut.RunProcess(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<ProcessSettings>(
                        p => p.Arguments.Render() == "/path:'c:/MyApp' /port:5555"));
        }

        [Theory, CustomAutoData(typeof(IISExpressRunnerCustomizations))]
        public void ShouldSetClrVersionSwitch([Frozen] IProcessRunner runner,
             IFileSystem fileSystem, AppPathBasedIISExpressRunner sut)
        {
            var settings = new AppPathBasedIISExpressSettings(@"c:\MyApp") { ClrVersion = ClrVersion.Version20 };
            fileSystem.Exist(settings.AppPath).Returns(true);

            sut.RunProcess(settings);

            runner.Received()
                .Start(Arg.Any<FilePath>(),
                    Arg.Is<ProcessSettings>(
                        p => p.Arguments.Render() == "/path:'c:/MyApp' /clr:v2.0"));
        }
    }
}