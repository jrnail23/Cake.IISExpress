using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

namespace Cake.IISExpress
{
    /// <summary>
    /// 
    /// </summary>
    [CakeAliasCategory("IIS Express")]
    public static class IISExpressAliases
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        [CakeMethodAlias]
        public static IProcess IISExpress(this ICakeContext context,
            ConfigBasedIISExpressSettings settings)
        {
            var runner = new ConfigBasedIISExpressRunner(context.FileSystem, context.Environment,
                context.ProcessRunner, context.Globber, context.Registry);

            return runner.RunProcess(settings);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        [CakeMethodAlias]
        public static IProcess IISExpress(this ICakeContext context,
            AppPathBasedIISExpressSettings settings)
        {
            var runner = new AppPathBasedIISExpressRunner(context.FileSystem, context.Environment,
                context.ProcessRunner, context.Globber, context.Registry);

            return runner.RunProcess(settings);
        }
    }
}