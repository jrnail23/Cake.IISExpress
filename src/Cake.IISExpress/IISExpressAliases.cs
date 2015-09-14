using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Process;

namespace Cake.IISExpress
{
    /// <summary>
    /// </summary>
    [CakeAliasCategory("IIS Express")]
    public static class IISExpressAliases
    {
        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        [CakeMethodAlias]
        public static IAdvProcess StartIISExpress(this ICakeContext context,
            ConfigBasedIISExpressSettings settings)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (settings == null)
                throw new ArgumentNullException("settings");

            var runner = new ConfigBasedIISExpressRunner(context.FileSystem, context.Environment,
                context.ProcessRunner, context.Globber, context.Registry, context.Log,
                new AdvProcessRunner(context.Environment, context.Log));

            return runner.StartServer(settings);
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        [CakeMethodAlias]
        public static IAdvProcess StartIISExpress(this ICakeContext context,
            AppPathBasedIISExpressSettings settings)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (settings == null)
                throw new ArgumentNullException("settings");

            var runner = new AppPathBasedIISExpressRunner(context.FileSystem, context.Environment,
                context.ProcessRunner, context.Globber, context.Registry, context.Log,
                new AdvProcessRunner(context.Environment, context.Log));

            return runner.StartServer(settings);
        }
    }
}