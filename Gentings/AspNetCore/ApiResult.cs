using System;

namespace Gentings.AspNetCore
{
    /// <summary>
    /// API结果。
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// 编码。
        /// </summary>
        public int Code { get; private set; }

        /// <summary>
        /// 设置错误编码。
        /// </summary>
        public Enum ErrorCode { set => Code = (int)(object)value; }

        /// <summary>
        /// 消息。
        /// </summary>
        public string Message { get; set; }
    }
}