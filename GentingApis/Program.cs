using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Gentings.Apis
{
    /// <summary>
    /// �������͡�
    /// </summary>
    public class Program
    {
        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="args">������</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// ��������������ʵ����
        /// </summary>
        /// <param name="args">������</param>
        /// <returns>���ط���������ʵ����</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
