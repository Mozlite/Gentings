using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gentings.AspNetCore
{
    /// <summary>
    /// 控制器基类。
    /// </summary>
    public abstract class ControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private Version _version;
        /// <summary>
        /// 当前程序的版本。
        /// </summary>
        public Version Version => _version ?? (_version = Cores.Version);

        private ILocalizer _localizer;
        /// <summary>
        /// 本地化接口。
        /// </summary>
        public ILocalizer Localizer => _localizer ?? (_localizer = GetRequiredService<ILocalizer>());

        private ILogger _logger;
        /// <summary>
        /// 日志接口。
        /// </summary>
        protected virtual ILogger Logger => _logger ?? (_logger = GetRequiredService<ILoggerFactory>().CreateLogger(GetType()));

        /// <summary>
        /// 获取注册的服务对象。
        /// </summary>
        /// <typeparam name="TService">服务类型或者接口。</typeparam>
        /// <returns>返回当前服务的实例对象。</returns>
        protected TService GetService<TService>() => HttpContext.RequestServices.GetService<TService>();

        /// <summary>
        /// 获取已经注册的服务对象。
        /// </summary>
        /// <typeparam name="TService">服务类型或者接口。</typeparam>
        /// <returns>返回当前服务的实例对象。</returns>
        protected TService GetRequiredService<TService>() => HttpContext.RequestServices.GetRequiredService<TService>();
    }
}