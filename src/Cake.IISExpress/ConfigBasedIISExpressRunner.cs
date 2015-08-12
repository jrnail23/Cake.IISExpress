using System;
using Cake.Core;
using Cake.Core.IO;

namespace Cake.IISExpress
{
    /// <summary>
    /// 
    /// </summary>
    public class ConfigBasedIISExpressRunner : IISExpressRunner<ConfigBasedIISExpressSettings>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="environment"></param>
        /// <param name="processRunner"></param>
        /// <param name="globber"></param>
        /// <param name="registry"></param>
        public ConfigBasedIISExpressRunner(IFileSystem fileSystem, ICakeEnvironment environment,
            IProcessRunner processRunner, IGlobber globber, IRegistry registry)
            : base(fileSystem, environment, processRunner, globber, registry)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public override IProcess RunProcess(ConfigBasedIISExpressSettings settings)
        {
            var arguments = new ProcessArgumentBuilder();

            var hasSiteNameToLaunch = !string.IsNullOrWhiteSpace(settings.SiteNameToLaunch);

            if (hasSiteNameToLaunch)
            {
                arguments.Append(string.Format("/site:'{0}'", settings.SiteNameToLaunch));
            }

            var hasSiteIdToLaunch = settings.SiteIdToLaunch.HasValue;

            if (hasSiteIdToLaunch)
            {
                arguments.Append(string.Format("/siteid:{0}", settings.SiteIdToLaunch));
            }

            if (hasSiteIdToLaunch && hasSiteNameToLaunch)
            {
                throw new InvalidOperationException(
                    "SiteName and SiteId should not be used together.");
            }

            if (settings.ConfigFilePath != null)
            {
                if (!FileSystem.Exist(settings.ConfigFilePath))
                {
                    throw new CakeException(
                        string.Format("IIS Express configuration file '{0}' does not exist.",
                            settings.ConfigFilePath));
                }

                arguments.Append(string.Format("/config:'{0}'",
                    settings.ConfigFilePath.MakeAbsolute(Environment)));
            }

            if (settings.TraceLevel != TraceLevel.None)
            {
                arguments.Append(string.Format("/trace:{0}",
                    settings.TraceLevel.ToString().ToLowerInvariant()));
            }

            if (!settings.EnableSystemTray)
            {
                arguments.Append("/systray:false");
            }

            return base.RunProcess(settings, arguments);
        }
    }
}