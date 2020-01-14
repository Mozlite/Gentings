using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Gentings.Apis
{
    /// <summary>
    /// 启动类型。
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 启动方法。
        /// </summary>
        /// <param name="args">参数。</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// 创建服务器宿主实例。
        /// </summary>
        /// <param name="args">参数。</param>
        /// <returns>返回服务器构建实例。</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
