using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Utilities;

namespace Cake.IISExpress
{
    /// <summary>
    ///     base class for IIS Express tool
    /// </summary>
    /// <typeparam name="TSettings">The type of the settings.</typeparam>
    public abstract class IISExpressRunner<TSettings> : Tool<TSettings>
        where TSettings : IISExpressSettings
    {
        private readonly ICakeEnvironment _cakeEnvironment;
        private readonly IFileSystem _fileSystem;
        private readonly ICakeLog _log;
        private readonly IRegistry _registry;

        /// <summary>
        /// Initializes a new instance of the <see cref="IISExpressRunner{TSettings}"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="processRunner">The process runner.</param>
        /// <param name="globber">The globber.</param>
        /// <param name="registry">The registry.</param>
        /// <param name="log">The log.</param>
        protected IISExpressRunner(IFileSystem fileSystem, ICakeEnvironment environment,
            IProcessRunner processRunner,
            IGlobber globber, IRegistry registry, ICakeLog log)
            : base(fileSystem, environment, processRunner, globber)
        {
            _registry = registry;
            _log = log;
            _cakeEnvironment = environment;
            _fileSystem = fileSystem;
        }

        /// <summary>
        ///     Gets the environment.
        /// </summary>
        /// <value>
        ///     The environment.
        /// </value>
        protected ICakeEnvironment Environment
        {
            get { return _cakeEnvironment; }
        }

        /// <summary>
        ///     Gets the file system.
        /// </summary>
        /// <value>
        ///     The file system.
        /// </value>
        protected IFileSystem FileSystem
        {
            get { return _fileSystem; }
        }

        /// <summary>
        ///     Gets the log.
        /// </summary>
        /// <value>
        ///     The log.
        /// </value>
        protected ICakeLog Log
        {
            get { return _log; }
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
        ///     Builds arguments specific to the implemented execution strategy
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        protected abstract ProcessArgumentBuilder BuildArguments(TSettings settings);

        /// <summary>
        ///     Runs the IIS Express process
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

            var process = base.RunProcess(settings, arguments, null, processSettings);

            process.ErrorDataReceived += (sender, args) =>
            {
                var errorMessage =
                    string.Format("IIS Express returned the following error message: '{0}'",
                        args.Data);
                throw new CakeException(errorMessage);
            };

            if (settings.WaitForStartup > 0)
            {
                // this supports a timeout on waiting for startup
                var stopwatch = Stopwatch.StartNew();
                var serverIsStarted = false;
                
                process.OutputDataReceived += (sender, args) =>
                {
                    if (!serverIsStarted &&
                        "IIS Express is running.".Equals(args.Data,
                            StringComparison.InvariantCultureIgnoreCase))
                    {
                        stopwatch.Stop();
                        serverIsStarted = true;
                    }
                };

                Log.Verbose("Waiting for IIS Express to start (timeout: {0}ms)",
                    settings.WaitForStartup);
                do
                {

                    if (stopwatch.ElapsedMilliseconds > settings.WaitForStartup)
                    {
                        throw new CakeException(string.Format(CultureInfo.CurrentCulture,
                            "Timed out while waiting for IIS Express to start. (timeout: {0}ms)",
                            settings.WaitForStartup));
                    }
                    
                    Thread.Sleep(10);
                } while (!serverIsStarted);

                Log.Information("IIS Express is running -- it took ~{0}ms to start.", stopwatch.ElapsedMilliseconds);
            }

            return process;
        }

        /// <summary>
        ///     Gets the tool executable names.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<string> GetToolExecutableNames()
        {
            return Enumerable.Empty<string>();
        }

        /// <summary>
        ///     Gets the alternative tool paths.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        protected override IEnumerable<FilePath> GetAlternativeToolPaths(TSettings settings)
        {
            var iisExpressRegistryKey =
                _registry.LocalMachine.OpenKey(@"SOFTWARE\Microsoft\IISExpress");

            if (iisExpressRegistryKey == null)
            {
                throw new CakeException("IIS Express is not installed on this machine.");
            }

            var latestVersion =
                iisExpressRegistryKey.GetSubKeyNames().OrderByDescending(decimal.Parse).First();

            var installPath =
                iisExpressRegistryKey.OpenKey(latestVersion).GetValue("InstallPath").ToString();

            var toolPath =
                DirectoryPath.FromString(installPath).CombineWithFilePath("iisexpress.exe");

            yield return toolPath;
        }
    }
}