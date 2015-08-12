using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Utilities;

namespace Cake.IISExpress
{

    /// <summary>
    /// base class for IIS Express tool 
    /// </summary>
    /// <typeparam name="TSettings">The type of the settings.</typeparam>
    public abstract class IISExpressRunner<TSettings> : Tool<TSettings>
        where TSettings : IISExpressSettings
    {
        private readonly IRegistry _registry;
        private readonly ICakeEnvironment _cakeEnvironment;
        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="environment"></param>
        /// <param name="processRunner"></param>
        /// <param name="globber"></param>
        /// <param name="registry"></param>
        protected IISExpressRunner(IFileSystem fileSystem, ICakeEnvironment environment,
            IProcessRunner processRunner,
            IGlobber globber, IRegistry registry)
            : base(fileSystem, environment, processRunner, globber)
        {
            _registry = registry;
            _cakeEnvironment = environment;
            _fileSystem = fileSystem;
        }

        /// <summary>
        /// 
        /// </summary>
        protected ICakeEnvironment Environment
        {
            get { return _cakeEnvironment; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected IFileSystem FileSystem
        {
            get { return _fileSystem; }
        }

        /// <summary>
        ///     Gets the name of the tool.
        /// </summary>
        /// <returns>The name of the tool.</returns>
        protected override string GetToolName()
        {
            return "IISExpress";
        }

        /// <summary>
        /// Runs the process.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        public abstract IProcess RunProcess(TSettings settings);

        /// <summary>
        /// Gets the tool executable names.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<string> GetToolExecutableNames()
        {
            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Gets the alternative tool paths.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        protected override IEnumerable<FilePath> GetAlternativeToolPaths(TSettings settings)
        {
            yield return
                DirectoryPath.FromString(
                    _registry.LocalMachine.OpenKey(@"SOFTWARE\Microsoft\IISExpress\8.0")
                        .GetValue("InstallPath").ToString()).CombineWithFilePath("iisexpress.exe");
        }
    }
}