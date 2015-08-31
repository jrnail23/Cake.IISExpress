using System;
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
        /// Gets the environment.
        /// </summary>
        /// <value>
        /// The environment.
        /// </value>
        protected ICakeEnvironment Environment
        {
            get { return _cakeEnvironment; }
        }

        /// <summary>
        /// Gets the file system.
        /// </summary>
        /// <value>
        /// The file system.
        /// </value>
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
        /// Builds arguments specific to the implemented execution strategy 
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        protected abstract ProcessArgumentBuilder BuildArguments(TSettings settings);

        /// <summary>
        /// Runs the IIS Express process
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual IProcess RunProcess(TSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            var arguments = BuildArguments(settings);

            if (settings.TraceLevel != TraceLevel.None)
            {
                arguments.Append("/trace:" + settings.TraceLevel.ToString().ToLowerInvariant());
            }

            if (!settings.EnableSystemTray)
            {
                arguments.Append("/systray:false");
            }

            var processSettings = new ProcessSettings
            {
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            return base.RunProcess(settings, arguments, null, processSettings);
        }

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
            var iisExpressRegistryKey = _registry.LocalMachine.OpenKey(@"SOFTWARE\Microsoft\IISExpress");
            var latestVersion =
                iisExpressRegistryKey.GetSubKeyNames().OrderByDescending(decimal.Parse).First();

            var installPath = iisExpressRegistryKey.OpenKey(latestVersion).GetValue("InstallPath").ToString();

            var toolPath =
                DirectoryPath.FromString(installPath).CombineWithFilePath("iisexpress.exe");

            yield return toolPath;
        }
    }
}