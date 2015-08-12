using System;
using Cake.Core.IO;
using NSubstitute;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;

namespace Cake.IISExpress.Tests
{
    public class IISExpressRunnerCustomizations : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<IRegistry>(
                cc => new TypeRelay(typeof (IRegistry), typeof (FakeIISExpressRegistry)));

            var fileSystemStub = Substitute.For<IFileSystem>();
            fixture.Inject(fileSystemStub);
            fileSystemStub.Exist(
                Arg.Is<FilePath>(
                    f =>
                        f.FullPath.Equals(@"c:/Program Files/IIS Express/IISExpress.exe",
                            StringComparison.OrdinalIgnoreCase)))
                .Returns(true);
        }
    }
}