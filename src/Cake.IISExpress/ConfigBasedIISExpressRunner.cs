using System;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Process;

namespace Cake.IISExpress
{
    /// <summary>
    /// </summary>
    public class ConfigBasedIISExpressRunner : IISExpressRunner<ConfigBasedIISExpressSettings>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigBasedIISExpressRunner" /> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="processRunner">The process runner.</param>
        /// <param name="globber">The globber.</param>
        /// <param name="registry">The registry.</param>
        /// <param name="log">The log.</param>
        /// <param name="advProcessRunner">The adv process runner.</param>
        public ConfigBasedIISExpressRunner(IFileSystem fileSystem, ICakeEnvironment environment,
            IProcessRunner processRunner, IGlobber globber, IRegistry registry, ICakeLog log,
            IAdvProcessRunner advProcessRunner)
            : base(fileSystem, environment, processRunner, globber, registry, log, advProcessRunner)
        {
        }

        /// <summary>
        ///     Builds arguments specific to the implemented execution strategy
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        protected override ProcessArgumentBuilder BuildArguments(
            ConfigBasedIISExpressSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            var arguments = new ProcessArgumentBuilder();

            var hasSiteNameToLaunch = !string.IsNullOrWhiteSpace(settings.SiteNameToLaunch);

            if (hasSiteNameToLaunch)
            {
                arguments.Append("/site:\"" + settings.SiteNameToLaunch + "\"");
            }

            var hasSiteIdToLaunch = settings.SiteIdToLaunch.HasValue;

            if (hasSiteIdToLaunch)
            {
                arguments.Append("/siteid:" + settings.SiteIdToLaunch);
            }

            if (hasSiteIdToLaunch && hasSiteNameToLaunch)
            {
                throw new InvalidOperationException(
                    "SiteName and SiteId should not be used together.");
            }

            if (settings.ConfigFilePath != null)
            {
                var absoluteConfigFilePath = settings.ConfigFilePath.MakeAbsolute(Environment);

                if (!FileSystem.Exist(absoluteConfigFilePath))
                {
                    throw new CakeException(
                        "IIS Express configuration file '" + absoluteConfigFilePath +
                        "' does not exist.");
                }

                arguments.Append("/config:\"" + absoluteConfigFilePath + "\"");
            }

            if (!string.IsNullOrEmpty(settings.AppPoolToLaunch))
            {
                arguments.Append("/apppool:" + settings.AppPoolToLaunch);
            }

            return arguments;
        }
    }
}